using System.Diagnostics;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Services;

public interface IStreamGrabberService
{
    Task<bool> GrabFrame(string url, string outputPath, CancellationToken ct = default);
}

public class StreamGrabberService : IStreamGrabberService
{
    private readonly AnalysisConfig _config;
    private readonly ActivitySource _activitySource;
    private readonly ILogger<StreamGrabberService> _logger;

    // resize to reduce token usage
    // constant from the majority of CCTV footage resolutions
    private const int MaximumWidth = 704;

    public StreamGrabberService(IOptions<AnalysisConfig> config, ILogger<StreamGrabberService> logger)
    {
        _logger = logger;
        _config = config.Value;

        _activitySource = new ActivitySource(typeof(StreamGrabberService).FullName!);
    }

    public async Task<bool> GrabFrame(string url, string outputPath, CancellationToken ct = default)
    {
        using var activity = _activitySource.StartActivity();
        var processInfo = new ProcessStartInfo
        {
            FileName = _config.FfmpegBinaryPath,
            Arguments = $"-i \"{url}\" -vf \"scale='min({MaximumWidth},iw):-2'\" -update true -frames:v 1 -q:v 2 -f image2pipe -",
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = Process.Start(processInfo);
        if (process == null)
        {
            return false;
        }
        
        var imageTask = Task.Run(async () =>
        {
            using var imageActivity = _activitySource.StartActivity("ImageWriter");
            await using var fs = File.OpenWrite(outputPath);    
            await process.StandardOutput.BaseStream.CopyToAsync(fs, ct);

            _logger.LogDebug("Frame grabbed and written {Bytes} bytes to {FilePath}", fs.Length, outputPath);
            return fs.Length > 0;
        }, ct);

        var outputTask = Task.Run(async () =>
        {
            using var logActivity = _activitySource.StartActivity("LogWriter");
            var errOutput = await process.StandardError.ReadToEndAsync(ct);
            
            _logger.LogDebug("Executed ffmpeg command.\n{FfmpegOutput}", errOutput);
        }, ct);

        await Task.WhenAll(imageTask, outputTask);

        return imageTask.Result;
    }
}
