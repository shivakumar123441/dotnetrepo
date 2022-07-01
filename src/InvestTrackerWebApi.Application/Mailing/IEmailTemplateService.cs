namespace InvestTrackerWebApi.Application.Mailing;

public interface IEmailTemplateService
{
    string GenerateEmailConfirmationMail(string userName, string email, string emailVerificationUri);
}
