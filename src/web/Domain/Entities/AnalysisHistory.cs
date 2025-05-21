using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;

public class AnalysisHistory
{
    public Guid Id { get; set; }
    public bool IsCameraOnline { get; set; }
    public string? Response { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public Guid CameraId { get; set; }
    public Guid? ResultId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Camera Camera { get; set; }
    public AnalysisResult? Result { get; set; }
}

public class AnalysisHistoryConfiguration : IEntityTypeConfiguration<AnalysisHistory>
{
    public void Configure(EntityTypeBuilder<AnalysisHistory> builder)
    {
        builder.HasOne(o => o.Camera)
            .WithMany(o => o.Histories)
            .HasForeignKey(o => o.CameraId);

        builder.HasOne(o => o.Result)
            .WithOne(o => o.History)
            .HasForeignKey<AnalysisHistory>(o => o.ResultId);
    }
}