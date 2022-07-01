namespace InvestTrackerWebApi.Application.Identity.Users;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using Microsoft.AspNetCore.Identity;

public class CreateUserCommandValidator : CustomValidator<CreateUserCommand>
{
    public CreateUserCommandValidator(UserManager<ApplicationUser> userManager)
    {
        // TODO

        //this.RuleFor(u => u.Email).Cascade(CascadeMode.Stop)
        //    .NotEmpty()
        //    .EmailAddress().WithMessage("Invalid Email Address.")
        //    .Must((email, _) => userManager.Users.Where(x => x.Email == email.Email).FirstOrDefault() is not null)
        //        .WithMessage((_, email) => string.Format("Email {0} is already registered.", email));

        //this.RuleFor(u => u.ReferralEmail).Cascade(CascadeMode.Stop)
        //    .EmailAddress()
        //    .WithMessage("Invalid Email Address.")
        //    .Must((email, _) => userManager.Users.Where(x => x.Email == email.ReferralEmail).FirstOrDefault() is null)
        //    .WithMessage((_, email) => string.Format("Email {0} is not registered.", email))
        //    .When(x => !string.IsNullOrEmpty(x.ReferralEmail));

        //this.RuleFor(u => u.UserName).Cascade(CascadeMode.Stop)
        //    .NotEmpty()
        //    .MinimumLength(6)
        //    .Must((email, _) => userManager.Users.Where(x => x.UserName == email.UserName).FirstOrDefault() is not null)
        //        .WithMessage((_, name) => string.Format("Username {0} is already taken.", name));

        this.RuleFor(p => p.FirstName).Cascade(CascadeMode.Stop)
            .NotEmpty();

        this.RuleFor(p => p.LastName).Cascade(CascadeMode.Stop)
            .NotEmpty();

        this.RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(6);

        this.RuleFor(p => p.ConfirmPassword).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Equal(p => p.Password);
    }
}
