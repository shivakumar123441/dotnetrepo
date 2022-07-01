namespace InvestTrackerWebApi.HttpApi.Notifications;
using InvestTrackerWebApi.Application.Notifications;
using Microsoft.AspNetCore.SignalR;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> notificationHubContext;

    public NotificationService(IHubContext<NotificationHub> notificationHubContext) =>
        this.notificationHubContext = notificationHubContext;

    public async Task BroadcastMessageAsync(INotificationMessage notification, CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.All.SendAsync(
            notification.MessageType, notification, cancellationToken);

    public async Task BroadcastExceptMessageAsync(
        INotificationMessage notification,
        IEnumerable<string> excludedConnectionIds,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.AllExcept(excludedConnectionIds)
        .SendAsync(notification.MessageType, notification, cancellationToken);

    public async Task SendMessageAsync(INotificationMessage notification, CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.Group($"Group").SendAsync(
            notification.MessageType, notification, cancellationToken);

    public async Task SendMessageExceptAsync(
        INotificationMessage notification,
        IEnumerable<string> excludedConnectionIds,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.GroupExcept($"Group", excludedConnectionIds)
        .SendAsync(notification.MessageType, notification, cancellationToken);

    public async Task SendMessageToGroupAsync(
        INotificationMessage notification,
        string group,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.Group(group).SendAsync(
            notification.MessageType, notification, cancellationToken);

    public async Task SendMessageToGroupsAsync(
        INotificationMessage notification,
        IEnumerable<string> groupNames,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.Groups(groupNames).SendAsync(
            notification.MessageType, notification, cancellationToken);

    public async Task SendMessageToGroupExceptAsync(
        INotificationMessage notification,
        string group,
        IEnumerable<string> excludedConnectionIds,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.GroupExcept(
            group,
            excludedConnectionIds)
        .SendAsync(notification.MessageType, notification, cancellationToken);

    public async Task SendMessageToUserAsync(
        string userId,
        INotificationMessage notification,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.User(userId).SendAsync(
            notification.MessageType, notification, cancellationToken);

    public async Task SendMessageToUsersAsync(
        IEnumerable<string> userIds,
        INotificationMessage notification,
        CancellationToken cancellationToken) =>
        await this.notificationHubContext.Clients.Users(userIds).SendAsync(
            notification.MessageType, notification, cancellationToken);
}
