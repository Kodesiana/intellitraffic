using System.Text.Json;
using System.Text.Json.Serialization;
using Kodesiana.BogorIntelliTraffic.Web.Domain.Enums;
using OpenAi.JsonSchema.Generator;
using OpenAi.JsonSchema.Serialization;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;

public class TrafficInfo
{
    private static readonly Lazy<string> JsonSchemaLazy = new(() =>
    {
        // https://github.com/r-Larch/OpenAi.JsonSchema
        var options = new JsonSchemaOptions(SchemaDefaults.OpenAi, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
        });

        var resolver = new DefaultSchemaGenerator();
        var schema = resolver.Generate<TrafficInfo>(options);

        return schema.ToJsonNode().ToJsonString();
    });

    public int TotalVehicles { get; set; }
    public bool HasCrowding { get; set; }
    public bool HasAccident { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TrafficDensity TrafficDensity { get; set; }

    public string Summary { get; set; }

    public static string JsonSchema => JsonSchemaLazy.Value;
}