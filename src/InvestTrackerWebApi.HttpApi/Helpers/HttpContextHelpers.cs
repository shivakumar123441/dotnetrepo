namespace InvestTrackerWebApi.HttpApi.Helpers;

using InvestTrackerWebApi.Application.Http;
using Microsoft.AspNetCore.Http;

public class HttpContextHelpers : IHttpContextHelpers
{

    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpContextHelpers(IHttpContextAccessor httpContextAccessor) =>
        this.httpContextAccessor = httpContextAccessor;

    public string GetOriginFromRequest() => $"{this.httpContextAccessor.HttpContext.Request.Scheme}://{this.httpContextAccessor.HttpContext.Request.Host.Value}{this.httpContextAccessor.HttpContext.Request.PathBase.Value}";

}
