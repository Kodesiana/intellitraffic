using Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;

public class Camera
{
    public Guid Id { get; set; }
    public string GivenName { get; set; }
    public string OverlayName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string HlsStreamName { get; set; }
    public string GoogleMapsUrl { get; set; }
    public VideoResolution VideoResolution { get; set; }
    public int VideoFrameRate { get; set; }
    public string VideoFormat { get; set; }

    public List<AnalysisResult> Results { get; set; }
    public List<AnalysisHistory> Histories { get; set; }
}