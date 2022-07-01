namespace InvestTrackerWebApi.Application.Notifications;

public class BasicNotification : INotificationMessage
{
    public enum LabelType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public string MessageType { get; set; } = nameof(BasicNotification);
    public string? Message { get; set; }
    public LabelType Label { get; set; }
}
