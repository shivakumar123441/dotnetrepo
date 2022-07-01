namespace InvestTrackerWebApi.DbMigrator.Application;
using InvestTrackerWebApi.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System.Reflection;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                args[0],
                npgsql =>
                {
                    _ = npgsql.MigrationsAssembly(typeof(ApplicationDbContextFactory)
                        .GetTypeInfo().Assembly.GetName().Name);
                    _ = npgsql.MigrationsHistoryTable("__Application_Migrations");
                });

        return new ApplicationDbContext(builder.Options, LoggerFactory.Create(builder => builder.AddConsole()));
    }
}
