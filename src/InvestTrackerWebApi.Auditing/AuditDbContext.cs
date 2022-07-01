namespace InvestTrackerWebApi.Auditing;

using System.Reflection;
using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AuditDbContext : DbContext, IAuditDbContext
{
    private readonly ILoggerFactory? loggerFactory;

    public AuditDbContext(DbContextOptions<AuditDbContext> options, ILoggerFactory? loggerFactory)
        : base(options) => this.loggerFactory = loggerFactory;

    public DbSet<Trail> AuditTrails => this.Set<Trail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseLoggerFactory(this.loggerFactory);
        _ = optionsBuilder.EnableSensitiveDataLogging();
    }
}
