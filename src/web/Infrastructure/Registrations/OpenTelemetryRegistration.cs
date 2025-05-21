using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Registrations;

public static class OpenTelemetryRegistration
{
    public const string GlobalVersion = "1.0.0";
    public const string GlobalMeterName = "Kodesiana.Apps.BogorIntelliTraffic";

    public static void AddCustomOpenTelemetry(this WebApplicationBuilder builder)
    {
        var otelEndpoint = builder.Configuration["OpenTelemetry:Endpoint"];
        var otel = builder.Services.AddOpenTelemetry();

        // Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource => resource
            .AddService(builder.Environment.ApplicationName));

        otel.WithMetrics(metrics =>
        {
            // Metrics provider from OpenTelemetry
            metrics.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                // Metrics provides by ASP.NET Core in .NET
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                // Metrics provided by System.Net libraries
                .AddMeter("System.Net.Http")
                .AddMeter("System.Net.NameResolution")
                // Metrics provided by App
                .AddMeter(GlobalMeterName);

            if (!string.IsNullOrWhiteSpace(otelEndpoint))
                metrics.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otelEndpoint + "/v1/metrics");
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
        });

        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation();

            if (!string.IsNullOrWhiteSpace(otelEndpoint))
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otelEndpoint + "/v1/traces");
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
        });

        otel.WithLogging(logging =>
        {
            if (!string.IsNullOrWhiteSpace(otelEndpoint))
                logging.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otelEndpoint + "/v1/logs");
                    otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
        });
    }
}
