using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.NotificationD
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? DataPayload { get; set; }
        public NotificationType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public int? AircraftId { get; set; }
        public DateTime? SentAtUtc { get; set; }
    }
}