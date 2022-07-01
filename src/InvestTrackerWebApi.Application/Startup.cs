namespace InvestTrackerWebApi.Application;
using System.Reflection;
using FluentValidation;
using InvestTrackerWebApi.Application.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        _ = services.AddMediatR(Assembly.GetExecutingAssembly());
        _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }
}
