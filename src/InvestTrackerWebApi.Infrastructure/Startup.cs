namespace InvestTrackerWebApi.Infrastructure;

using System.Reflection;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.PostgreSql;
using HangfireBasicAuthenticationFilter;
using InvestTrackerWebApi.Application.BackgroundJobs;
using InvestTrackerWebApi.Application.Caching;
using InvestTrackerWebApi.Application.Events;
using InvestTrackerWebApi.Application.ImageStorage;
using InvestTrackerWebApi.Application.Mailing;
using InvestTrackerWebApi.Application.Serialization;
using InvestTrackerWebApi.Domain.Configurations;
using InvestTrackerWebApi.Infrastructure.Caching;
using InvestTrackerWebApi.Infrastructure.ImageStorage;
using InvestTrackerWebApi.Infrastructure.HangFire;
using InvestTrackerWebApi.Infrastructure.Mailing;
using InvestTrackerWebApi.Infrastructure.Serialization;
using InvestTrackerWebApi.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MediatR;
using InvestTrackerWebApi.Application.FileStorage;
using InvestTrackerWebApi.Infrastructure.FileStorage;
using InvestTrackerWebApi.Application.Exporters;
using InvestTrackerWebApi.Infrastructure.Exporters;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConnectionStrings connectionStrings,
        HangfireSettings hangfireSettings)
    {
        _ = services.AddTransient<IImageStorageService, ImgurStorageService>();
        _ = services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        _ = services.AddTransient<IEventService, EventService>();
        _ = services.AddTransient<ISerializerService, NewtonSoftService>();
        _ = services.AddTransient<IBackgroundJobService, BackgroundJobService>();
        _ = services.AddTransient<IMailService, SmtpMailService>();
        _ = services.AddTransient<IEmailTemplateService, EmailTemplateService>();
        _ = services.AddTransient<IExcelWriter, ExcelWriter>();

        _ = services.AddMemoryCache();
        _ = services.AddTransient<ICacheService, LocalCacheService>();
        _ = services.AddScoped<ICacheKeyService, CacheKeyService>();

        _ = services.AddHangfireConsoleExtensions();

        _ = services.AddHangfire((_, config) => config.UsePostgreSqlStorage(
            connectionStrings.DefaultConnection,
            new PostgreSqlStorageOptions()
            {
                QueuePollInterval = hangfireSettings.QueuePollInterval,
                InvisibilityTimeout = hangfireSettings.InvisibilityTimeout
            })
            .UseFilter(new LogJobFilter())
            .UseConsole());

        _ = services.AddHangfireServer(options =>
        {
            options.HeartbeatInterval = hangfireSettings.HeartbeatInterval;
            options.Queues = hangfireSettings.Queues?.ToArray();
            options.SchedulePollingInterval = hangfireSettings.SchedulePollingInterval;
            options.ServerCheckInterval = hangfireSettings.ServerCheckInterval;
            options.ServerName = hangfireSettings.ServerName;
            options.ServerTimeout = hangfireSettings.ServerTimeout;
            options.ShutdownTimeout = hangfireSettings.ShutdownTimeout;
            options.WorkerCount = hangfireSettings.WorkerCount;
        });

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, HangfireSettings hangfireSettings)
    {
        _ = app.UseHangfireDashboard(hangfireSettings.Route, new DashboardOptions
        {
            DashboardTitle = hangfireSettings.DashboardTitle,
            StatsPollingInterval = hangfireSettings.StatsPollingInterval,
            AppPath = hangfireSettings.AppPath,
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = hangfireSettings.UserName,
                    Pass = hangfireSettings.Password
                }
            }
        });

        return app;
    }
}

