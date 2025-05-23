namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Configuration;

public class AnalysisConfig
{
    public int MaxParallelism { get; set; }
    public int IntervalInMinutes { get; set; }
    public string SystemPrompt { get; set; }
    public string SnapshotDirectory { get; set; }
    public string HlsStreamingFormatUri { get; set; }
    public string WebRTCStreamingFormatUri { get; set; }
    public string FfmpegBinaryPath { get; set; }
}
