namespace InvestTrackerWebApi.Application.Identity.Roles;

using System.Security.Claims;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class UpdateRolePermissionsCommand : IRequest<string>
{
    public Guid RoleId { get; set; }
    public List<string> Permissions { get; set; } = default!;
}

public class UpdateRolePermissionsCommandHandler : IRequestHandler<UpdateRolePermissionsCommand, string>
{
    private readonly RoleManager<ApplicationRole> roleManager;

    public UpdateRolePermissionsCommandHandler(RoleManager<ApplicationRole> roleManager) =>
        this.roleManager = roleManager;

    public async Task<string> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await this.roleManager.FindByIdAsync(request.RoleId.ToString());

        _ = role ?? throw new NotFoundException("Role Not Found");

        if (role.Name.ToLowerInvariant() == RootConstants.RootAdminRole.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

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

        return "Permissions Updated.";
    }
}
