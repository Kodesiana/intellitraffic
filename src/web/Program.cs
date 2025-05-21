using System.Text.Json;
using Kodesiana.BogorIntelliTraffic.Web.Components;
using Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;
using Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;
using Kodesiana.BogorIntelliTraffic.Web.Domain.Workers;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Configuration;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Database;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Registrations;
using Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OpenAI.Chat;

/* --- SCAFFOLDING --- */
var builder = WebApplication.CreateBuilder(args);

// Configure options pattern
builder.Services.Configure<AnalysisConfig>(builder.Configuration.GetSection("Analysis"));

// Add OpenTelemetry
builder.AddCustomOpenTelemetry();

// Add Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add EF Core
builder.Services.AddDbContextFactory<BogorContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BogorContext"))
        .UseSeeding((context, _) =>
        {
            var hasCamera = context.Set<Camera>().Any();
            if (hasCamera || !File.Exists("cameras.jsonl"))
            {
                return;
            }
            
            var lines = File.ReadAllLines("cameras.jsonl");
            var cameras = lines.Select(x => JsonSerializer.Deserialize<CameraSeed>(x))
                .Where(x => x is not null)
                .Select(x => new Camera
                {
                    Id = Guid.CreateVersion7(),
                    GivenName = x.Name,
                    OverlayName = x.CctvName,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    HlsStreamName = Path.GetFileNameWithoutExtension(x.StreamUrl),
                    GoogleMapsUrl = x.GmapUrl,
                    VideoResolution = VideoResolution.Parse(x.VideoRes),
                    VideoFrameRate = x.VideoFps,
                    VideoFormat = x.VideoCodec
                });

            context.Set<Camera>().AddRange(cameras);
            context.SaveChanges();
        })
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging());

// Add services
builder.Services.AddSingleton<IStreamGrabberService, StreamGrabberService>();
builder.Services.AddSingleton<ChatClient>(_ =>
{
    var config = builder.Configuration.GetSection("OpenAI");
    var model = config.GetValue<string>("Model");
    var apiKey = config.GetValue<string>("ApiKey");
    
    return new ChatClient(model, apiKey);
});

// Add Hosted Services
builder.Services.AddHostedService<PeriodicVideoAnalysisWorker>();

/* --- MIDDLEWARE AND PATH MAPPING --- */
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/snapshots",
    FileProvider = new PhysicalFileProvider(builder.Configuration["Analysis:SnapshotDirectory"]!),
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={60 * 60 * 24 * 7}");
    }
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
