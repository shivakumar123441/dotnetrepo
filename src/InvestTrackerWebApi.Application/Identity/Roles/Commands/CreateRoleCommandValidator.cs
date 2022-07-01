namespace InvestTrackerWebApi.Application.Identity.Roles;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationRole;
using Microsoft.AspNetCore.Identity;

public class CreateRoleCommandValidator : CustomValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleManager<ApplicationRole> roleManager) =>
        this.RuleFor(r => r.Name)
            .NotEmpty();
}
