namespace InvestTrackerWebApi.Application.Identity.Roles;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class UpdateRolePermissionsCommandValidator : CustomValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        this.RuleFor(r => r.RoleId)
            .NotEmpty();
        this.RuleFor(r => r.Permissions)
            .NotNull();
    }
}
