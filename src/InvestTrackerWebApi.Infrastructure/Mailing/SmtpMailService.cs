namespace InvestTrackerWebApi.Infrastructure.Mailing;
using InvestTrackerWebApi.Application.Mailing;
using InvestTrackerWebApi.Domain.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

public class SmtpMailService : IMailService
{
    private readonly MailSettings mailSettings;
    private readonly ILogger<SmtpMailService> logger;

    public SmtpMailService(IOptions<MailSettings> settings, ILogger<SmtpMailService> logger)
    {
        this.mailSettings = settings.Value;
        this.logger = logger;
    }

    public async Task SendAsync(MailRequest request)
    {
        try
        {
            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(this.mailSettings.DisplayName, request.From ?? this.mailSettings.From));

            // To
            foreach (string address in request.To)
            {
                email.To.Add(MailboxAddress.Parse(address));
            }

            // Reply To
            if (!string.IsNullOrEmpty(request.ReplyTo))
            {
                email.ReplyTo.Add(new MailboxAddress(request.ReplyToName, request.ReplyTo));
            }

            // Bcc
            if (request.Bcc != null)
            {
                foreach (string address in request.Bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    email.Bcc.Add(MailboxAddress.Parse(address.Trim()));
                }
            }

            // Cc
            if (request.Cc != null)
            {
                foreach (string? address in request.Cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    email.Cc.Add(MailboxAddress.Parse(address.Trim()));
                }
            }

            // Headers
            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    email.Headers.Add(header.Key, header.Value);
                }
            }

            // Content
            var builder = new BodyBuilder();
            email.Sender = new MailboxAddress(
                request.DisplayName ?? this.mailSettings.DisplayName, request.From ?? this.mailSettings.From);
            email.Subject = request.Subject;
            builder.HtmlBody = request.Body;

            // Create the file attachments for this e-mail message
            if (request.AttachmentData != null)
            {
                foreach (var attachmentInfo in request.AttachmentData)
                {
                    _ = builder.Attachments.Add(attachmentInfo.Key, attachmentInfo.Value);
                }
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(this.mailSettings.Host, this.mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(this.mailSettings.UserName, this.mailSettings.Password);
            _ = await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
        }
    }
}
