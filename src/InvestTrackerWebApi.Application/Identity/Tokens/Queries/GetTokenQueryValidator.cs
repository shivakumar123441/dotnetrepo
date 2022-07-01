namespace InvestTrackerWebApi.Application.Identity.Tokens;

using FluentValidation;
using InvestTrackerWebApi.Application.Validation;

public class GetTokenQueryValidator : CustomValidator<GetTokenQuery>
{
    public GetTokenQueryValidator()
    {
        this.RuleFor(p => p.Email).Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
                .WithMessage("Invalid Email Address.");

        this.RuleFor(p => p.Password).Cascade(CascadeMode.Stop)
            .NotEmpty();
    }
}
