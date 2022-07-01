namespace InvestTrackerWebApi.Application.Accounts;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class CreateReferralAccountCommandValidator : CustomValidator<CreateReferralAccountCommand>
{
    public CreateReferralAccountCommandValidator() => _ = this.RuleFor(p => p.UserId).NotEmpty();
}
