
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Application.NotificationD;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;
using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AircraftMRO.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();


            // Map to DTO (prevents traversal of Aircraft/WorkOrders/EmailNotifications)
            var dto = new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                DataPayload = notification.DataPayload,
                Type = notification.Type,
                Severity = notification.Severity,
                AircraftId = notification.AircraftId,
                SentAtUtc = notification.SentAtUtc
            };

            if (notification.Channel == NotificationChannel.InApp || notification.Channel == NotificationChannel.Both)
            {
                // If it's a Grounded event, broadcast to everyone
                if (notification.Type == NotificationType.AircraftGrounded)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", dto);
                }
                else
                {
                    // Otherwise, send to specific user
                    await _hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", dto);
                }
            }
        }
    }
}