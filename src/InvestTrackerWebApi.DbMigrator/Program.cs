using System.Reflection;
using InvestTrackerWebApi.Auditing;
using InvestTrackerWebApi.DbMigrator.Application;
using InvestTrackerWebApi.DbMigrator.Identity;
using InvestTrackerWebApi.DbMigrator.Identity.SeedData;
using InvestTrackerWebApi.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InvestTrackerWebApi.DbMigrator.Application.SeedData;
using Microsoft.Extensions.Logging;
using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Application.Serialization;
using InvestTrackerWebApi.Infrastructure.Services;
using InvestTrackerWebApi.Infrastructure.Serialization;
using MediatR;
using InvestTrackerWebApi.DbMigrator.Audit;

var services = new ServiceCollection();
services.AddLogging(configure =>
{
    configure.SetMinimumLevel(LogLevel.Information);
    configure.AddConsole();
});

_ = services.AddMediatR(Assembly.GetExecutingAssembly());
_ = services.AddTransient<IEventService, EventService>();
_ = services.AddTransient<ISerializerService, NewtonSoftService>();

services.AddDbContext<AuditDbContext>(options => options.UseNpgsql(
    args[0],
    npgsql =>
    {
        _ = npgsql.MigrationsAssembly(typeof(AuditDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
        _ = npgsql.MigrationsHistoryTable("__Audit_Migrations", "audit");
    }));

services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(
    args[0],
    npgsql =>
    {
        _ = npgsql.MigrationsAssembly(typeof(IdentityDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
        _ = npgsql.MigrationsHistoryTable("__Identity_Migrations", "identity");
    }));

services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
    args[0],
    npgsql =>
    {
        _ = npgsql.MigrationsAssembly(typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
        _ = npgsql.MigrationsHistoryTable("__Application_Migrations");
    }));

_ = services.AddScoped<IAuditDbContext>(provider => provider.GetRequiredService<AuditDbContext>());
_ = services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 2;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

services.AddTransient<IdentityDataSeeder>();
services.AddTransient<InvestmentsDataSeeder>();

var serviceProvider = services.BuildServiceProvider();

using (var migrationScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using var identityDbContext = migrationScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await identityDbContext.Database.MigrateAsync();

    using var applicationDbContext = migrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await applicationDbContext.Database.MigrateAsync();

    using var auditDbContext = migrationScope.ServiceProvider.GetRequiredService<AuditDbContext>();
    await auditDbContext.Database.MigrateAsync();
}

using (var seedScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var identityDataSeeder = seedScope.ServiceProvider.GetRequiredService<IdentityDataSeeder>();
    await identityDataSeeder.SeedDatabaseAsync();

    var investmentsDataSeeder = seedScope.ServiceProvider.GetRequiredService<InvestmentsDataSeeder>();
    await investmentsDataSeeder.SeedDatabaseAsync();
}

