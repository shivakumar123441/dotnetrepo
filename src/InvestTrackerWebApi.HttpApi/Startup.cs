namespace InvestTrackerWebApi.HttpApi;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Http;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Application.Notifications;
using InvestTrackerWebApi.Domain.Configurations;
using InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.HttpApi.Filters;
using InvestTrackerWebApi.HttpApi.Helpers;
using InvestTrackerWebApi.HttpApi.Middleware;
using InvestTrackerWebApi.HttpApi.Notifications;
using InvestTrackerWebApi.HttpApi.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using NSwag.Generation.Processors.Security;
using ZymLabs.NSwag.FluentValidation;

public static class Startup
{
    public static IServiceCollection AddHttpApi(
        this IServiceCollection services,
        JwtSettings jwtSettings,
        CorsSettings corsSettings,
        SwaggerSettings swaggerSettings)
    {
        if (string.IsNullOrEmpty(jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Key);

        _ = services.AddAuthentication(authentication =>
        {
            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(bearer =>
        {
            bearer.RequireHttpsMetadata = false;
            bearer.SaveToken = true;
            bearer.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            bearer.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        throw new UnauthorizedAccessException("Authentication Failed.");
                    }

                    return Task.CompletedTask;
                },
                OnForbidden = _ => throw new ForbiddenException("You are not authorized to access this resource."),
            };
        });

        _ = services.AddHttpContextAccessor();
        _ = services.AddTransient<IHttpContextHelpers, HttpContextHelpers>();
        _ = services.AddScoped<ICurrentUser, CurrentUser>();
        _ = services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

        _ = services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        _ = services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        _ = services.AddScoped<CurrentUserMiddleware>();
        _ = services.AddScoped<RequestLoggingMiddleware>();
        _ = services.AddScoped<ResponseLoggingMiddleware>();
        _ = services.AddScoped<ExceptionMiddleware>();

        _ = services.AddSignalR();
        _ = services.AddTransient<NotificationHub>();
        _ = services.AddTransient<INotificationService, NotificationService>();

        _ = services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        _ = services.AddRouting(options => options.LowercaseUrls = true);
        _ = services.AddControllers(opts => opts.Filters.Add(typeof(ModelStateFeatureFilter)))
            .AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter() { NamingStrategy = new DefaultNamingStrategy() });
                opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opts.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            })
            .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .AddFluentValidation(x => x.AutomaticValidationEnabled = false);
        _ = services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        _ = services.AddVersionedApiExplorer(options => options.SubstituteApiVersionInUrl = true);
        _ = services.AddEndpointsApiExplorer();

        _ = services.AddCors(opt => opt.AddPolicy(
            corsSettings.Policy!,
            policy =>
            policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials()));

        _ = services.AddOpenApiDocument((document, serviceProvider) =>
          {
              document.PostProcess = doc =>
              {
                  doc.Info.Title = swaggerSettings.Title;
                  doc.Info.Version = swaggerSettings.Version;
                  doc.Info.Description = swaggerSettings.Description;
              };

              _ = document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
              {
                  Name = "Authorization",
                  Description = "Input your Bearer token to access this API",
                  In = OpenApiSecurityApiKeyLocation.Header,
                  Type = OpenApiSecuritySchemeType.Http,
                  Scheme = JwtBearerDefaults.AuthenticationScheme,
                  BearerFormat = "JWT",
              });

              document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
              document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());
              document.OperationProcessors.Add(new OrderableEnumOperationProcessor());
              document.OperationProcessors.Add(new InnerFilterPropertiesOperationProcessor());

              document.TypeMappers.Add(new PrimitiveTypeMapper(typeof(TimeSpan), schema =>
              {
                  schema.Type = NJsonSchema.JsonObjectType.String;
                  schema.IsNullableRaw = true;
                  schema.Pattern =
                  @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$";
                  schema.Example = "02:00:00";
              }));

              var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();
              document.SchemaProcessors.Add(fluentValidationSchemaProcessor);
          });

        _ = services.AddScoped<FluentValidationSchemaProcessor>();

        return services;
    }

    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
        app.UseMiddleware<CurrentUserMiddleware>();

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        _ = app.UseMiddleware<RequestLoggingMiddleware>();
        _ = app.UseMiddleware<ResponseLoggingMiddleware>();

        return app;
    }

    public static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        _ = endpoints.MapHub<NotificationHub>(
            "/notifications",
            options => options.CloseOnAuthenticationExpiration = true);
        return endpoints;
    }

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();
}
