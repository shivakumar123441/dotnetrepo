namespace InvestTrackerWebApi.Auditing;

using InvestTrackerWebApi.Application.Auditing;
using InvestTrackerWebApi.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class Startup
{
    public static IServiceCollection AddAuditing(
        this IServiceCollection services,
        ConnectionStrings connectionStrings)
    {
        _ = services.AddDbContext<AuditDbContext>(options => options.UseNpgsql(connectionStrings.DefaultConnection!));

        _ = services.AddScoped<IAuditDbContext>(provider => provider.GetRequiredService<AuditDbContext>());

        return services;
    }
}
