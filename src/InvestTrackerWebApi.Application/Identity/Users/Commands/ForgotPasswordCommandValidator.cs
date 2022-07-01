namespace InvestTrackerWebApi.Application.Identity.Users;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class ForgotPasswordCommandValidator : CustomValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator() =>
        this.RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");
}
