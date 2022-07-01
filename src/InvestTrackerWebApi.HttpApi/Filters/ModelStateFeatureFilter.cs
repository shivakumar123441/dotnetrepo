namespace InvestTrackerWebApi.HttpApi.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

public class ModelStateFeatureFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var state = context.ModelState;
        context.HttpContext.Features.Set(new ModelStateFeature(state));
        await next();
    }
}
