
using System.Text.Json;
using AircraftMRO.Application.Events;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.Handlers
{
    public class AircraftGroundedEventHandler : MediatR.INotificationHandler<AircraftGroundedEvent>
    {
        private readonly INotificationService _notificationService;


        public AircraftGroundedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(AircraftGroundedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendNotificationAsync(new Notification
            {
                // IMPORTANT: Since we are using Clients.All for AircraftGrounded, 
                UserId = "System",
                Type = NotificationType.AircraftGrounded,
                Channel = NotificationChannel.Both,
                Title = notification.Title,
                Message = $"Aircraft {notification.AircraftId} Grounded.",
                Severity = AlertSeverity.Critical,
                AircraftId = notification.AircraftId,
                DataPayload = JsonSerializer.Serialize(new
                {
                    aircraftId = notification.AircraftId,
                    tailNumber = notification.TailNumber, 
                    groundedAt = DateTime.UtcNow, 
                    status = "Grounded",
                    url = $"/aircraft/{notification.AircraftId}"
                })
            });
        }
    }
}