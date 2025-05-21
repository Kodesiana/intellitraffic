using System.Diagnostics;
using System.Diagnostics.Metrics;
using Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;
using Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Configuration;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Database;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Services;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using OTEL = Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Registrations.OpenTelemetryRegistration;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.Workers;

public class PeriodicVideoAnalysisWorker : BackgroundService
{
    private readonly AnalysisConfig _config;

    private readonly ActivitySource _activitySource;
    private readonly Counter<long> _inputTokensCounter;

    private readonly ILogger<PeriodicVideoAnalysisWorker> _logger;
    private readonly ChatClient _openaiClient;
    private readonly Counter<long> _outputTokensCounter;
    private readonly SemaphoreSlim _semaphore;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IStreamGrabberService _streamGrabberService;
    private readonly PeriodicTimer _timer;

    public PeriodicVideoAnalysisWorker(IServiceScopeFactory serviceScopeFactory,
        ChatClient openaiClient, IStreamGrabberService streamGrabberService,
        ILogger<PeriodicVideoAnalysisWorker> logger, IOptions<AnalysisConfig> config, IMeterFactory meterFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _openaiClient = openaiClient;
        _streamGrabberService = streamGrabberService;
        _logger = logger;
        _config = config.Value;

        _timer = new PeriodicTimer(TimeSpan.FromMinutes(_config.IntervalInMinutes));
        _semaphore = new SemaphoreSlim(_config.MaxParallelism, _config.MaxParallelism);

        var meter = meterFactory.Create(OTEL.GlobalMeterName, OTEL.GlobalVersion);
        _inputTokensCounter = meter.CreateCounter<long>("llm.tokens.input");
        _outputTokensCounter = meter.CreateCounter<long>("llm.tokens.output");
        _activitySource = new ActivitySource(typeof(PeriodicVideoAnalysisWorker).FullName!);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            using var activity = _activitySource.StartActivity();

            try
            {
                _logger.LogInformation("Starting periodic video analysis");
                await Process(stoppingToken);
                _logger.LogInformation("Finished periodic video analysis");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to execute periodic video analysis");
                activity?.SetStatus(ActivityStatusCode.Error, e.Message);
            }
        }
    }

    private async Task Process(CancellationToken ct)
    {
        using var activity = _activitySource.StartActivity();
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<BogorContext>();

        var cameras = await context.Cameras.AsNoTracking().ToListAsync(ct);
        _logger.LogInformation("Found {CameraCount} cameras", cameras.Count);

        _logger.LogInformation("Starting video analysis");
        var results = await Task.WhenAll(cameras.Select(c => ProcessCamera(c, ct)));

        foreach (var item in results)
        {
            // insert history
            var history = new AnalysisHistory
            {
                Id = Guid.CreateVersion7(),
                CameraId = item.cameraId,
                CreatedAt = DateTime.UtcNow
            };

            if (item.inputTokens > 0)
            {
                history.Response = item.jsonOutput;
                history.InputTokens = item.inputTokens;
                history.OutputTokens = item.outputTokens;
            }

            var parsed = JsonHelper.DeserializeOpenAI<TrafficInfo>(item.jsonOutput);
            if (parsed is not null)
            {
                history.IsCameraOnline = true;
                history.Result = new AnalysisResult
                {
                    Id = Guid.CreateVersion7(),
                    TotalVehicles = parsed.TotalVehicles,
                    HasCrowding = parsed.HasCrowding,
                    HasAccident = parsed.HasAccident,
                    TrafficDensity = parsed.TrafficDensity,
                    Summary = parsed.Summary,
                    SnapshotUrl = item.snapshotUrl,
                    CameraId = item.cameraId,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Frame from camera {CameraId} analyzed! Currently the traffic is {Density}",
                    history.CameraId, parsed.TrafficDensity);
            }
            else
            {
                history.IsCameraOnline = false;
                _logger.LogWarning("Failed to analyze camera {CameraId} frame", item.cameraId);
            }

            await context.AnalysisHistories.AddAsync(history, ct);

            // update latest results
            if (history.Result is null) continue;

            if (await context.LatestAnalyses.AnyAsync(x => x.CameraId == item.cameraId, ct))
            {
                var latest = await context.LatestAnalyses.SingleAsync(x => x.CameraId == item.cameraId, ct);
                latest.ResultId = history.Result.Id;
            }
            else
            {
                await context.LatestAnalyses.AddAsync(new LatestResult
                {
                    Id = Guid.CreateVersion7(),
                    CameraId = item.cameraId,
                    ResultId = history.Result.Id,
                    LastUpdate = DateTime.UtcNow,
                }, ct);
            }
        }

        await context.SaveChangesAsync(ct);
    }

    private async Task<(Guid cameraId, string? jsonOutput, int inputTokens, int outputTokens, string? snapshotUrl)>
        ProcessCamera(
            Camera camera,
            CancellationToken ct)
    {
        _logger.LogInformation("Task started for camera {CameraId}. Waiting for semaphore lock...", camera.Id);
        await _semaphore.WaitAsync(ct);

        using var activity = _activitySource.StartActivity();
        
        try
        {
            // create URL
            var streamingUrl = string.Format(_config.HlsStreamingFormatUri, camera.HlsStreamName);
            _logger.LogInformation("Start processing frame for camera {CameraId}", camera.Id);

            // create file
            var now = DateTime.UtcNow;
            var fileName = now.ToString("HHmmss") + ".jpg";
            var savePath = Path.Join(_config.SnapshotDirectory, camera.Id.ToString(), now.ToString("yyyyMMdd"),
                fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            // grab frame
            _logger.LogInformation("Camera {CameraId} health is OK. Capturing frame from {URL}...", camera.Id,
                streamingUrl);
            var grabSuccess = await _streamGrabberService.GrabFrame(streamingUrl, savePath, ct);
            if (!grabSuccess)
            {
                _logger.LogError("Failed to ping frame for camera {CameraId}. Aborting.", camera.Id);
                return (camera.Id, null, 0, 0, null);
            }

            // call LLM
            _logger.LogInformation("Frame captured from camera {CameraId}. Calling LLM...", camera.Id);
            var completion = await AnalyzeFile(savePath, ct);

            _logger.LogInformation("Used a total of {TotalTokens} when analyzing camera {CameraId} frame",
                completion.inputTokens + completion.outputTokens, camera.Id);

            var snapshotUrl = $"/snapshots/{camera.Id.ToString()}/{now:yyyyMMdd}/{fileName}";
            return (camera.Id, completion.content, completion.inputTokens, completion.outputTokens, snapshotUrl);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to process camera data");
            return (camera.Id, null, 0, 0, null);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<(string content, int inputTokens, int outputTokens)> AnalyzeFile(string path,
        CancellationToken ct)
    {
        using var activity = _activitySource.StartActivity();
        activity?.SetTag("path", path);

        // https://github.com/openai/openai-dotnet/tree/main?tab=readme-ov-file#how-to-use-chat-completions-with-structured-outputs
        var fileContents = await BinaryData.FromStreamAsync(File.OpenRead(path), ct);
        List<ChatMessage> messages =
        [
            ChatMessage.CreateSystemMessage(_config.SystemPrompt),
            ChatMessage.CreateUserMessage(
                ChatMessageContentPart.CreateTextPart("Analyze this image"),
                ChatMessageContentPart.CreateImagePart(fileContents, "image/jpg"))
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "traffic_analysis",
                BinaryData.FromString(TrafficInfo.JsonSchema),
                jsonSchemaIsStrict: true)
        };

        var result = await _openaiClient.CompleteChatAsync(messages, options, ct);
        _inputTokensCounter.Add(result.Value.Usage.InputTokenCount);
        _outputTokensCounter.Add(result.Value.Usage.OutputTokenCount);

        return (result.Value.Content[0].Text, result.Value.Usage.InputTokenCount, result.Value.Usage.OutputTokenCount);
    }
}
