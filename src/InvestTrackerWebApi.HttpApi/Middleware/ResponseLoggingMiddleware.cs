namespace InvestTrackerWebApi.HttpApi.Middleware;
using InvestTrackerWebApi.Application.Identity.Users;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

public class ResponseLoggingMiddleware : IMiddleware
{
    private readonly ICurrentUser currentUser;

    public ResponseLoggingMiddleware(ICurrentUser currentUser) =>
        this.currentUser = currentUser;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        await next(httpContext);
        var originalBody = httpContext.Response.Body;
        using var newBody = new MemoryStream();
        httpContext.Response.Body = newBody;
        string responseBody;
        if (httpContext.Request.Path.ToString().Contains("tokens"))
        {
            responseBody = "[Redacted] Contains Sensitive Information.";
        }
        else
        {
            _ = newBody.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        }

        string email = this.currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
        var userId = this.currentUser.GetUserId();
        if (userId != Guid.Empty)
        {
            _ = LogContext.PushProperty("UserId", userId);
        }

        _ = LogContext.PushProperty("UserEmail", email);
        _ = LogContext.PushProperty("StatusCode", httpContext.Response.StatusCode);
        _ = LogContext.PushProperty("ResponseTimeUTC", DateTime.UtcNow);
        Log.ForContext(
            "ResponseHeaders",
            httpContext.Response.Headers.ToDictionary(
                h => h.Key, h => h.Value.ToString()),
            destructureObjects: true)
       .ForContext("ResponseBody", responseBody)
       .Information(
            "HTTP {RequestMethod} Request to {RequestPath} by {RequesterEmail} has Status Code {StatusCode}.",
            httpContext.Request.Method,
            httpContext.Request.Path,
            email,
            httpContext.Response.StatusCode);
        _ = newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}
