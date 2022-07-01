namespace InvestTrackerWebApi.Application.Identity.Users;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class ChangeCurrentUserPasswordCommandValidator : CustomValidator<ChangeCurrentUserPasswordCommand>
{
    public ChangeCurrentUserPasswordCommandValidator()
    {
        this.RuleFor(p => p.Password)
            .NotEmpty();

        this.RuleFor(p => p.NewPassword)
            .NotEmpty();

        this.RuleFor(p => p.ConfirmNewPassword)
            .Equal(p => p.NewPassword)
                .WithMessage("Passwords do not match.");
    }
}
