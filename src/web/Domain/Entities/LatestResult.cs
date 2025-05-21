using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;

public class LatestResult
{
    public Guid Id { get; set; }
    public Guid CameraId { get; set; }
    public Guid ResultId { get; set; }
    public DateTime LastUpdate { get; set; }

    public Camera Camera { get; set; }
    public AnalysisResult Result { get; set; }
}

public class LatestResultConfiguration : IEntityTypeConfiguration<LatestResult>
{
    public void Configure(EntityTypeBuilder<LatestResult> builder)
    {
        builder.HasOne(x => x.Camera)
            .WithOne()
            .HasForeignKey<LatestResult>(x => x.CameraId);

        builder.HasOne(x => x.Result)
            .WithOne()
            .HasForeignKey<LatestResult>(x => x.ResultId);
    }
}