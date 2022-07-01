namespace InvestTrackerWebApi.Persistence;

using InvestTrackerWebApi.Application.Persistence;
using InvestTrackerWebApi.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class Startup
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        ConnectionStrings connectionStrings)
    {
        _ = services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionStrings.DefaultConnection!));

        _ = services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
