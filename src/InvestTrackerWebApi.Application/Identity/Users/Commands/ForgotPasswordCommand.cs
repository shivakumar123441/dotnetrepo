namespace InvestTrackerWebApi.Application.Identity.Users;

using InvestTrackerWebApi.Application.BackgroundJobs;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Http;
using InvestTrackerWebApi.Application.Mailing;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

public class ForgotPasswordCommand : IRequest<string>
{
    public string Email { get; set; } = default!;
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IBackgroundJobService backgroundJobService;
    private readonly IMailService mailService;
    private readonly IHttpContextHelpers httpContextHelpers;

    public ForgotPasswordCommandHandler(UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService, IMailService mailService, IHttpContextHelpers httpContextHelpers)
    {
        this.userManager = userManager;
        this.backgroundJobService = backgroundJobService;
        this.mailService = mailService;
        this.httpContextHelpers = httpContextHelpers;
    }

    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await this.userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await this.userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new IdentityException("An Error has occurred!");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await this.userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{this.httpContextHelpers.GetOriginFromRequest()}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest(
            new List<string> { request.Email },
            "Reset Password",
            $"Your Password Reset Token is '{code}'." +
            $" You can reset your password using the {endpointUri} Endpoint.");
        _ = this.backgroundJobService.Enqueue(() => this.mailService.SendAsync(mailRequest));

        return "Password Reset Mail has been sent to your authorized Email.";
    }
}
