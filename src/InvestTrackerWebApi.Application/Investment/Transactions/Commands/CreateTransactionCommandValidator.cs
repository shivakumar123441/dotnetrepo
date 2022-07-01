namespace InvestTrackerWebApi.Application.Transactions;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class CreateTransactionCommandValidator : CustomValidator<CreateTransactionCommand>
{
    //TODO
    //public CreateTransactionCommandValidator() =>
    //    _ = this.RuleFor(p => p.TransactionType).NotEmpty();
}
