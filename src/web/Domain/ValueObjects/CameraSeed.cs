using System.Text.Json.Serialization;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;

public class CameraSeed
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    [JsonPropertyName("longitude")] public double Longitude { get; set; }

    [JsonPropertyName("stream_url")] public string StreamUrl { get; set; }

    [JsonPropertyName("gmap_url")] public string GmapUrl { get; set; }

    [JsonPropertyName("cctv_name")] public string CctvName { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; }

    [JsonPropertyName("video_res")] public string VideoRes { get; set; }

    [JsonPropertyName("video_fps")] public int VideoFps { get; set; }

    [JsonPropertyName("video_codec")] public string VideoCodec { get; set; }

    [JsonPropertyName("owner")] public string Owner { get; set; }
}