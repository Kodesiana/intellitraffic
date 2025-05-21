namespace Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;

public readonly record struct VideoResolution(int Width, int Height)
{
    public static VideoResolution Parse(string value)
    {
        var parts = value.Split('x');
        if (int.TryParse(parts[0], out var w) && int.TryParse(parts[1], out var h)) return new VideoResolution(w, h);

        throw new FormatException("Invalid video resolution format");
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }
}