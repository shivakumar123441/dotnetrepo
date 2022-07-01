namespace InvestTrackerWebApi.Application.Accounts;
using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class UpdateAccountCommandValidator : CustomValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator() => _ = this.RuleFor(p => p.Description)
            .NotEmpty()
            .MaximumLength(75);
}
