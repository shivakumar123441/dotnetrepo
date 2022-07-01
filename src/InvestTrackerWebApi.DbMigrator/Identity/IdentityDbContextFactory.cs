namespace InvestTrackerWebApi.DbMigrator.Identity;
using InvestTrackerWebApi.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System.Reflection;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(
                args[0],
                npgsql =>
                {
                    _ = npgsql.MigrationsAssembly(typeof(IdentityDbContextFactory)
                        .GetTypeInfo().Assembly.GetName().Name);
                    _ = npgsql.MigrationsHistoryTable("__Identity_Migrations", "identity");
                });

        return new IdentityDbContext(builder.Options, LoggerFactory.Create(builder => builder.AddConsole()));
    }
}
