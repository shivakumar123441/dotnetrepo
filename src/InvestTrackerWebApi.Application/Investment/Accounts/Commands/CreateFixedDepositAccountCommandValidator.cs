namespace InvestTrackerWebApi.Application.Accounts;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class CreateFixedDepositAccountCommandValidator : CustomValidator<CreateFixedDepositAccountCommand>
{
    public CreateFixedDepositAccountCommandValidator() => _ = this.RuleFor(p => p.UserId).NotEmpty();
}
