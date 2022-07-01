namespace InvestTrackerWebApi.HttpApi.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> logger;

    public NotificationHub(ILogger<NotificationHub> logger) => this.logger = logger;

    public override async Task OnConnectedAsync()
    {
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, $"Group");
        await base.OnConnectedAsync();
        this.logger.LogInformation($"A client connected to NotificationHub: {this.Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, $"Group");
        await base.OnDisconnectedAsync(exception);
        this.logger.LogInformation($"A client disconnected from NotificationHub: {this.Context.ConnectionId}");
    }
}
