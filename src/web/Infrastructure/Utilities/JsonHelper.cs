using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Utilities;

public static class JsonHelper
{
    public static T? DeserializeOpenAI<T>(string? json)
    {
        try
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }
        catch (Exception)
        {
            return default;
        }
    }
}