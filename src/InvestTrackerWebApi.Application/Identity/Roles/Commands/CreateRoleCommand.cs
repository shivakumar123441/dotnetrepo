namespace InvestTrackerWebApi.Application.Identity.Roles;

using System.Security.Claims;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class CreateRoleCommand : IRequest<string>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = default!;
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, string>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly ICurrentUser currentUser;

    public CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager, ICurrentUser currentUser)
    {
        this.roleManager = roleManager;
        this.currentUser = currentUser;
    }

    public async Task<string> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Create a new role.
        var role = new ApplicationRole(request.Name, request.Description, this.currentUser.GetUserId());
        role.AddDomainEvent(new ApplicationRoleCreatedEvent(role));
        var result = await this.roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            throw new ValidationException(
                "Validation Errors Occurred.",
                result.Errors.GroupBy(e => e.Code, e => e.Description)
                    .ToDictionary(
                        failureGroup =>
                        failureGroup.Key,
                        failureGroup => failureGroup.ToArray()));
        }

        // Add permissions to role
        if (request.Permissions != null && request.Permissions.Count > 0)
        {
            var currentClaims = await this.roleManager.GetClaimsAsync(role);

            role.AddDomainEvent(new ApplicationRoleUpdatedEvent(role));

            // Remove permissions that were previously selected
            foreach (var claim in currentClaims.Where(c => !request.Permissions.Any(p => p == c.Value)))
            {
                var removeResult = await this.roleManager.RemoveClaimAsync(role, claim);
                if (!removeResult.Succeeded)
                {
                    throw new ValidationException(
                        "Validation Errors Occurred.",
                        removeResult.Errors.GroupBy(e => e.Code, e => e.Description)
                            .ToDictionary(
                                failureGroup =>
                                failureGroup.Key,
                                failureGroup => failureGroup.ToArray()));
                }
            }

            // Add all permissions that were not previously selected
            foreach (string permission in request.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
            {
                var addResult = await this.roleManager.AddClaimAsync(
                        role,
                        new Claim(Domain.Identity.ClaimTypes.Permission, permission));

                if (!addResult.Succeeded)
                {
                    throw new ValidationException(
                        "Validation Errors Occurred.",
                        addResult.Errors.GroupBy(e => e.Code, e => e.Description)
                            .ToDictionary(
                                failureGroup =>
                                failureGroup.Key,
                                failureGroup => failureGroup.ToArray()));
                }
            }
        }

        return string.Format("Role {0} Created.", request.Name);
    }
}
