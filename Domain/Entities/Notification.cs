
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        // Core content
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? DataPayload { get; set; }

        // Categorization (Replacing Alert properties)
        public NotificationType Type { get; set; } // Handles "Grounded", "OverdueWorkOrder", etc.
        public NotificationChannel Channel { get; set; }
        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;

        // Status tracking
        public bool IsRead { get; set; }
        public DateTime? ResolvedAt { get; set; } // Replaces Alert.ResolvedAt
        public DateTime? SentAtUtc { get; set; }
        public bool IsEmailProcessed { get; set; }



        // Relationship replacement
        public int? AircraftId { get; set; }
        [ForeignKey(nameof(AircraftId))]
        public Aircraft? Aircraft { get; set; }



        public ICollection<EmailNotification> EmailNotifications { get; set; } = new List<EmailNotification>();
    }
}