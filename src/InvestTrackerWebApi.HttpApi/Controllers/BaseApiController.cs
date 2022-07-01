namespace InvestTrackerWebApi.HttpApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[ApiController]
public class BaseApiController : ControllerBase
{
    private ISender mediator = null!;

    protected ISender Mediator => this.mediator ??= this.HttpContext.RequestServices.GetRequiredService<ISender>();

}
