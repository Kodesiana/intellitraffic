using Kodesiana.BogorIntelliTraffic.Web.Domain.Entities;
using Kodesiana.BogorIntelliTraffic.Web.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Kodesiana.BogorIntelliTraffic.Web.Infrastructure.Database;

public class BogorContext : DbContext
{
    public BogorContext(DbContextOptions<BogorContext> options) : base(options)
    {
    }

    public DbSet<Camera> Cameras { get; set; }
    public DbSet<AnalysisResult> AnalysisResults { get; set; }
    public DbSet<AnalysisHistory> AnalysisHistories { get; set; }
    public DbSet<LatestResult> LatestAnalyses { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<VideoResolution>(t => t.HaveConversion<VideoResolutionConverter>());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalysisHistoryConfiguration).Assembly);
    }
}