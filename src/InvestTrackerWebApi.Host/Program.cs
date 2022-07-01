using InvestTrackerWebApi.Application;
using InvestTrackerWebApi.Host.Extensions;
using InvestTrackerWebApi.HttpApi;
using InvestTrackerWebApi.Infrastructure;
using InvestTrackerWebApi.Persistence;
using InvestTrackerWebApi.Identity;
using Serilog;
using Microsoft.AspNetCore.Rewrite;
using InvestTrackerWebApi.Auditing;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    _ = builder.Host.ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        _ = config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    });

    _ = builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

    var jwtSettings = builder.Services.LoadJwtSettings(builder.Configuration);
    var connectionStrings = builder.Services.LoadConnectionStrings(builder.Configuration);
    var corsSettings = builder.Services.LoadCorsSettings(builder.Configuration);
    var hangfireSettings = builder.Services.LoadHangfireSettings(builder.Configuration);
    var mailSettings = builder.Services.LoadMailSettings(builder.Configuration);
    var swaggerSettings = builder.Services.LoadSwaggerSettings(builder.Configuration);

    _ = builder.Services.AddApplication();
    _ = builder.Services.AddInfrastructure(connectionStrings, hangfireSettings);
    _ = builder.Services.AddIdentity(connectionStrings);
    _ = builder.Services.AddHttpApi(jwtSettings, corsSettings, swaggerSettings);
    _ = builder.Services.AddAuditing(connectionStrings);
    _ = builder.Services.AddPersistence(connectionStrings);

    var app = builder.Build();

    _ = app.UseExceptionMiddleware();
    _ = app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
    _ = app.UseHttpsRedirection();
    _ = app.UseStaticFiles();
    _ = app.UseRouting();
    _ = app.UseCors(corsSettings.Policy!);
    _ = app.UseAuthentication();
    _ = app.UseCurrentUser();
    _ = app.UseAuthorization();
    _ = app.UseRequestLogging();
    _ = app.UseHangfireDashboard(hangfireSettings);
    _ = app.UseOpenApi();
    _ = app.UseSwaggerUi3(options =>
    {
        options.DefaultModelsExpandDepth = -1;
        options.DocExpansion = "none";
        options.TagsSorter = "alpha";
    });
    _ = app.UseEndpoints(endpoints =>
    {
        _ = endpoints.MapControllers().RequireAuthorization();
        _ = endpoints.MapNotifications();
    });

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}

