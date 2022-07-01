namespace InvestTrackerWebApi.Identity;

using InvestTrackerWebApi.Application.Identity;
using InvestTrackerWebApi.Domain.Configurations;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class Startup
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, ConnectionStrings connectionStrings)
    {
        _ = services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionStrings.DefaultConnection!));

        _ = services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
          {
              options.Password.RequiredLength = 2;
              options.Password.RequireDigit = false;
              options.Password.RequireLowercase = false;
              options.Password.RequireNonAlphanumeric = false;
              options.Password.RequireUppercase = false;
              options.User.RequireUniqueEmail = true;
          }).AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        _ = services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());

        return services;
    }
}

