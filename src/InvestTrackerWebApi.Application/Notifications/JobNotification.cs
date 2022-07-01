namespace InvestTrackerWebApi.Application.Notifications;

public class JobNotification : INotificationMessage
{
    public string MessageType { get; set; } = nameof(JobNotification);
    public string? Message { get; set; }
    public string? JobId { get; set; }
    public decimal Progress { get; set; }
}
