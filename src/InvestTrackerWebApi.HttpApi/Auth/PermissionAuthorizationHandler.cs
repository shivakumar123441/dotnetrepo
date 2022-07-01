namespace InvestTrackerWebApi.HttpApi.Auth;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.HttpApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IMediator mediator;

    public PermissionAuthorizationHandler(IMediator mediator) =>
        this.mediator = mediator;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.GetUserId() is { } userId)
        {
            var permissions = await this.mediator.Send(new GetCurrentUserPermissionsQuery());
            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
