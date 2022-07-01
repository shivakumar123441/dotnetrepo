namespace InvestTrackerWebApi.Host.Extensions;
using InvestTrackerWebApi.Domain.Configurations;

public static class ServiceExtensions
{
    public static JwtSettings LoadJwtSettings(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettingsConfigSection = config.GetSection($"{nameof(JwtSettings)}");
        _ = services.Configure<JwtSettings>(jwtSettingsConfigSection);
        return jwtSettingsConfigSection.Get<JwtSettings>();
    }

    public static ConnectionStrings LoadConnectionStrings(this IServiceCollection services, IConfiguration config)
    {
        var connectionStringsConfigSection = config.GetSection($"{nameof(ConnectionStrings)}");
        _ = services.Configure<ConnectionStrings>(connectionStringsConfigSection);
        return connectionStringsConfigSection.Get<ConnectionStrings>();
    }

    public static CorsSettings LoadCorsSettings(this IServiceCollection services, IConfiguration config)
    {
        var corsSettingsConfigSection = config.GetSection($"{nameof(CorsSettings)}");
        _ = services.Configure<CorsSettings>(corsSettingsConfigSection);
        return corsSettingsConfigSection.Get<CorsSettings>();
    }

    public static HangfireSettings LoadHangfireSettings(this IServiceCollection services, IConfiguration config)
    {
        var hangfireSettingsConfigSection = config.GetSection($"{nameof(HangfireSettings)}");
        _ = services.Configure<HangfireSettings>(hangfireSettingsConfigSection);
        return hangfireSettingsConfigSection.Get<HangfireSettings>();
    }

    public static MailSettings LoadMailSettings(this IServiceCollection services, IConfiguration config)
    {
        var mailSettingsConfigSection = config.GetSection($"{nameof(MailSettings)}");
        _ = services.Configure<MailSettings>(mailSettingsConfigSection);
        return mailSettingsConfigSection.Get<MailSettings>();
    }

    public static SwaggerSettings LoadSwaggerSettings(this IServiceCollection services, IConfiguration config)
    {
        var swaggerSettingsConfigSection = config.GetSection($"{nameof(SwaggerSettings)}");
        _ = services.Configure<SwaggerSettings>(swaggerSettingsConfigSection);
        return swaggerSettingsConfigSection.Get<SwaggerSettings>();
    }
}
