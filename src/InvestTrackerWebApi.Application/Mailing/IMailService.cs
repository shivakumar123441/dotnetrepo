namespace InvestTrackerWebApi.Application.Mailing;

public interface IMailService
{
    Task SendAsync(MailRequest request);
}
