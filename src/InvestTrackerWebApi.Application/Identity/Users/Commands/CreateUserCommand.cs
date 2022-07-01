namespace InvestTrackerWebApi.Application.Identity.Users;

using System.Text;
using InvestTrackerWebApi.Application.BackgroundJobs;
using InvestTrackerWebApi.Application.Exceptions;
using InvestTrackerWebApi.Application.Http;
using InvestTrackerWebApi.Application.Mailing;
using InvestTrackerWebApi.Domain.Identity;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser;
using InvestTrackerWebApi.Domain.Identity.Entities.ApplicationUser.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

public class CreateUserCommand : IRequest<string>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ReferralEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IBackgroundJobService backgroundJobService;
    private readonly ICurrentUser currentUser;
    private readonly IMailService mailService;
    private readonly IEmailTemplateService templateService;
    private readonly IHttpContextHelpers httpContextHelpers;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService, ICurrentUser currentUser, IMailService mailService, IEmailTemplateService templateService, IHttpContextHelpers httpContextHelpers)
    {
        this.userManager = userManager;
        this.backgroundJobService = backgroundJobService;
        this.currentUser = currentUser;
        this.mailService = mailService;
        this.templateService = templateService;
        this.httpContextHelpers = httpContextHelpers;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser(request.Email, request.FirstName, request.LastName, request.ReferralEmail, request.UserName, request.PhoneNumber, this.currentUser.GetUserId());
        user.AddDomainEvent(new ApplicationUserCreatedEvent(user));
        var result = await this.userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new ValidationException(
                "Validation Errors Occurred.",
                result.Errors.GroupBy(e => e.Code, e => e.Description)
                    .ToDictionary(
                    failureGroup =>
                    failureGroup.Key,
                    failureGroup => failureGroup.ToArray()));
        }

        user.AddDomainEvent(new ApplicationUserUpdatedEvent(user));
        _ = await this.userManager.AddToRoleAsync(user, DefaultRoles.Basic);

        var messages = new List<string> { string.Format("User {0} Registered.", user.UserName) };

        if (!string.IsNullOrEmpty(user.Email))
        {
            // send verification email
            string emailVerificationUri = await this.GetEmailVerificationUriAsync(user);
            var mailRequest = new MailRequest(
                new List<string> { user.Email },
                "Confirm Registration",
                this.templateService.GenerateEmailConfirmationMail(
                    user.UserName ?? "User",
                    user.Email,
                    emailVerificationUri));
            _ = this.backgroundJobService.Enqueue(() => this.mailService.SendAsync(mailRequest));
            messages.Add($"Please check {user.Email} to verify your account!");
        }

        return string.Join(Environment.NewLine, messages);
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user)
    {
        string code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "api/users/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{this.httpContextHelpers.GetOriginFromRequest()}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryConstants.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryConstants.Code, code);
        return verificationUri;
    }
}
