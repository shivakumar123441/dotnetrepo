namespace InvestTrackerWebApi.HttpApi.Middleware;

using System.Net;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Serialization;
using InvestTrackerWebApi.HttpApi.Filters;
using InvestTrackerWebApi.HttpApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;
using Serilog.Context;

internal class ExceptionMiddleware : IMiddleware
{
    private readonly ICurrentUser currentUser;
    private readonly ISerializerService jsonSerializer;
    private readonly IDictionary<Type, Func<Guid, Exception, HttpContext, Task>> exceptionHandlers;

    public ExceptionMiddleware(
        ICurrentUser currentUser,
        ISerializerService jsonSerializer)
    {
        this.currentUser = currentUser;
        this.jsonSerializer = jsonSerializer;
        this.exceptionHandlers = new Dictionary<Type, Func<Guid, Exception, HttpContext, Task>>
        {
            { typeof(ValidationException), this.HandleValidationExceptionAsync },
            { typeof(NotFoundException), this.HandleNotFoundExceptionAsync },
            { typeof(UnauthorizedAccessException), this.HandleUnauthorizedAccessExceptionAsync },
            { typeof(ForbiddenException), this.HandleForbiddenExceptionAsync },
            { typeof(IdentityException), this.HandleIdentityExceptionAsync },
        };
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            string email = this.currentUser.GetUserEmail() is string userEmail ? userEmail : "Anonymous";
            var userId = this.currentUser.GetUserId();
            Guid errorId = Guid.NewGuid();

            if (userId != Guid.Empty)
            {
                LogContext.PushProperty("UserId", userId);
            }

            LogContext.PushProperty("UserEmail", email);
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("Source", exception.TargetSite?.DeclaringType?.FullName);
            LogContext.PushProperty("StackTrace", exception.StackTrace);

            var modelState = context.Features.Get<ModelStateFeature>()?.ModelState;

            if (modelState != null)
            {
                if (!modelState.IsValid)
                {
                    await this.HandleInvalidModelStateExceptionAsync(errorId, modelState, context);
                    return;
                }
            }

            await this.HandleExceptionAsync(errorId, exception, context);
        }
    }

    private async Task HandleInvalidModelStateExceptionAsync(
        Guid errorId,
        ModelStateDictionary modelState,
        HttpContext httpContext)
    {
        var validationErrorsResult = new ValidationErrorsDto
        {
            ExceptionMessage = "Request model binding did not happen properly.",
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
            ValidationErrors = modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()!)
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.BadRequest;

        Log.Error($"Request model binding did not happen properly. Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(validationErrorsResult));
    }

    private async Task HandleExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        Type type = exception.GetType();
        if (this.exceptionHandlers.ContainsKey(type))
        {
            await this.exceptionHandlers[type].Invoke(errorId, exception, httpContext);
            return;
        }

        await this.HandleUnknownExceptionAsync(errorId, exception, httpContext);
    }

    private async Task HandleValidationExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var validationException = exception as ValidationException;
        var validationErrorsResult = new ValidationErrorsDto
        {
            ExceptionMessage = validationException?.Message.Trim()!,
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
            ValidationErrors = validationException?.ErrorMessages!
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.BadRequest;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(validationErrorsResult));
    }

    private async Task HandleNotFoundExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var notFoundException = exception as NotFoundException;
        var errorResult = new ErrorDto
        {
            ExceptionMessage = notFoundException?.Message.Trim()!,
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.NotFound;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(errorResult));
    }

    private async Task HandleUnauthorizedAccessExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var unauthorizedAccessException = exception as UnauthorizedAccessException;
        var errorResult = new ErrorDto
        {
            ExceptionMessage = unauthorizedAccessException?.Message.Trim()!,
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.Unauthorized;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(errorResult));
    }

    private async Task HandleForbiddenExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var forbiddenException = exception as ForbiddenException;
        var errorResult = new ErrorDto
        {
            ExceptionMessage = forbiddenException?.Message.Trim()!,
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.Forbidden;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(errorResult));
    }

    private async Task HandleIdentityExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var identityException = exception as IdentityException;
        var errorResult = new ErrorDto
        {
            ExceptionMessage = identityException?.Message.Trim()!,
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.InternalServerError;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(errorResult));
    }

    private async Task HandleUnknownExceptionAsync(Guid errorId, Exception exception, HttpContext httpContext)
    {
        var errorResult = new ErrorDto
        {
            ExceptionMessage = exception.Message.Trim(),
            ErrorId = errorId,
            SupportMessage = "Provide the ErrorId to the support team for further analysis.",
        };

        var response = httpContext.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.InternalServerError;

        Log.Error($"{exception.Message.Trim()} Request failed with Status Code " +
                $"{response.StatusCode} and Error Id {errorId}.");

        await response.WriteAsync(this.jsonSerializer.Serialize(errorResult));
    }
}
