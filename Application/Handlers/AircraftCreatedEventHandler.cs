
using System.Text.Json;
using AircraftMRO.Application.Events;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.Handlers
{
    public class AircraftCreatedEventHandler : MediatR.INotificationHandler<AircraftCreatedEvent>
    {
        private readonly INotificationService _notificationService;


        public AircraftCreatedEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(AircraftCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendNotificationAsync(new Notification
            {
                // IMPORTANT: Since we are using Clients.All for AircraftGrounded, 
                UserId = "System",
                Type = NotificationType.AircraftGrounded,
                Channel = NotificationChannel.Both,
                Title = notification.Title,
                Message = $"Aircraft {notification.AircraftId} Grounded.",
                DataPayload = JsonSerializer.Serialize(new
                {
                    aircraftId = notification.AircraftId,
                    status = "Grounded",
                    url = $"/aircraft/{notification.AircraftId}"
                })
            });
        }
    }
}