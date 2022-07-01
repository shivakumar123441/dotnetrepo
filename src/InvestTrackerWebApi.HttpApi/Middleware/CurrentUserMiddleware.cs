namespace InvestTrackerWebApi.HttpApi.Middleware;

using InvestTrackerWebApi.HttpApi.Auth;
using Microsoft.AspNetCore.Http;

public class CurrentUserMiddleware : IMiddleware
{
    private readonly ICurrentUserInitializer currentUserInitializer;

    public CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) =>
        this.currentUserInitializer = currentUserInitializer;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        this.currentUserInitializer.SetCurrentUser(context.User);

        await next(context);
    }
}
