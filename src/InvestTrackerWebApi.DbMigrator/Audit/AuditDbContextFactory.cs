namespace InvestTrackerWebApi.DbMigrator.Audit;
using InvestTrackerWebApi.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System.Reflection;

public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AuditDbContext>()
            .UseNpgsql(
                args[0],
                npgsql =>
                {
                    _ = npgsql.MigrationsAssembly(typeof(AuditDbContextFactory)
                        .GetTypeInfo().Assembly.GetName().Name);
                    _ = npgsql.MigrationsHistoryTable("__Audit_Migrations", "audit");
                });

        return new AuditDbContext(builder.Options, LoggerFactory.Create(builder => builder.AddConsole()));
    }
}
