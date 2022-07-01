namespace InvestTrackerWebApi.Application.Notifications;

public class StatsChangedNotification : INotificationMessage
{
    public string MessageType { get; set; } = nameof(StatsChangedNotification);
    public string? Message { get; set; }
}
