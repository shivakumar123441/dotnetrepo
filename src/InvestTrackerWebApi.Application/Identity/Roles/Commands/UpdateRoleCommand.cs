namespace InvestTrackerWebApi.Application.Identity.Roles;

using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Identity.Users;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

public class UpdateRoleCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, string>
{
    private readonly RoleManager<ApplicationRole> roleManager;
    private readonly ICurrentUser currentUser;

    public UpdateRoleCommandHandler(RoleManager<ApplicationRole> roleManager, ICurrentUser currentUser)
    {
        this.roleManager = roleManager;
        this.currentUser = currentUser;
    }

    public async Task<string> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // Update an existing role.
        var role = await this.roleManager.FindByIdAsync(request.Id.ToString());

        _ = role ?? throw new NotFoundException("Role Not Found");

        if (role.Name.ToLowerInvariant() == RootConstants.RootAdminRole.ToLowerInvariant())
        {
            throw new IdentityException("Operation not allowed.");
        }

        role = role.Update(request.Name, request.Description, this.currentUser.GetUserId());

        role.AddDomainEvent(new ApplicationRoleUpdatedEvent(role));

        var result = await this.roleManager.UpdateAsync(role);

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

        return string.Format("Role {0} Updated.", role.Name);
    }
}
