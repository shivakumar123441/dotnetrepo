namespace InvestTrackerWebApi.Application.Accounts;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class CreateWalletAccountCommandValidator : CustomValidator<CreateWalletAccountCommand>
{
    public CreateWalletAccountCommandValidator() => _ = this.RuleFor(p => p.UserId).NotEmpty();
}
