using Kodesiana.BogorIntelliTraffic.Web.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;

public class AnalysisResult
{
    public Guid Id { get; set; }
    public int TotalVehicles { get; set; }
    public bool HasCrowding { get; set; }
    public bool HasAccident { get; set; }
    public TrafficDensity TrafficDensity { get; set; }
    public string Summary { get; set; }
    public string SnapshotUrl { get; set; }
    public Guid CameraId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camera Camera { get; set; }
    public AnalysisHistory History { get; set; }
}

public class ActivityConfiguration : IEntityTypeConfiguration<AnalysisResult>
{
    public void Configure(EntityTypeBuilder<AnalysisResult> builder)
    {
        builder.HasOne(o => o.Camera)
            .WithMany(o => o.Results)
            .HasForeignKey(o => o.CameraId);
    }
}