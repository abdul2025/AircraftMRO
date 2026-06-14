using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Domain.Entities
{
    public class EmailNotification
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }

        [ForeignKey(nameof(NotificationId))]
        public Notification Notification { get; set; } = null!;

        public string RecipientEmail { get; set; } = string.Empty;
        public DateTime? SentAtUtc { get; set; }
        public string? ErrorMessage { get; set; }

        
    }
}