using MediatR;

namespace AircraftMRO.Application.Events
{
    public class SendEmailEvent : INotification
    {
        public int EmailNotificationId { get; set; }
        
    }
}