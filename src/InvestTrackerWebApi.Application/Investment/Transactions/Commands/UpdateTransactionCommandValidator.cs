namespace InvestTrackerWebApi.Application.Transactions;
using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class UpdateTransactionCommandValidator : CustomValidator<UpdateTransactionCommand>
{
    //TODO
    //public UpdateTransactionCommandValidator() => _ = this.RuleFor(p => p.Name)
    //        .NotEmpty()
    //        .MaximumLength(75);
}
