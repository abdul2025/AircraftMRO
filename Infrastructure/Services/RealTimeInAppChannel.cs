using AircraftMRO.Models.Enums;
using AircraftMRO.Services.Interfaces.Notification;
using Microsoft.AspNetCore.SignalR;

namespace AircraftMRO.Infrastructure.Services
{
    public class RealTimeInAppChannel : INotificationChannel
    {
        public NotificationChannelType ChannelType => NotificationChannelType.InApp;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeInAppChannel(IHubContext<NotificationHub> _hubContext)
        {
            this._hubContext = _hubContext;
        }

        public async Task<bool> SendAsync(string userId, string title, string message, string? recipientTarget = null)
        {
            try
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new { title, message });
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}