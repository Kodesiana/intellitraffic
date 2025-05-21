using Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Database;

public class VideoResolutionConverter : ValueConverter<VideoResolution, string>
{
    public VideoResolutionConverter()
        : base(
            v => $"{v.Width}x{v.Height}",
            v => VideoResolution.Parse(v))
    {
    }
}