using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AircraftMRO.Application.Events;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.Handlers
{
    public class WorkOrderOverdueEventHandler : MediatR.INotificationHandler<WorkOrderOverdueEvent>
    {
        private readonly INotificationService _notificationService;

        public WorkOrderOverdueEventHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(WorkOrderOverdueEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendNotificationAsync(new Notification
            {
                UserId = "System",
                Type = NotificationType.OverdueWorkOrder,
                Channel = NotificationChannel.Both,
                Title = "Overdue Work Order",
                Message = $"Work Order {notification.WorkOrderId} for Aircraft {notification.TailNumber} is overdue.",
                Severity = AlertSeverity.Warning,
                AircraftId = notification.AircraftId,
                DataPayload = JsonSerializer.Serialize(new
                {
                    workOrderId = notification.WorkOrderId,
                    aircraftId = notification.AircraftId,
                    tailNumber = notification.TailNumber, 
                    description = $"Work Order {notification.WorkOrderId} is overdue", 
                    url = $"/workorders/{notification.WorkOrderId}"
                })
            });
        }
    }
}