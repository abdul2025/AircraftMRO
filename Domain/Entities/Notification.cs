
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public bool IsRead { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? DataPayload { get; set; }
        public DateTime? SentAtUtc { get; set; }
    }
}