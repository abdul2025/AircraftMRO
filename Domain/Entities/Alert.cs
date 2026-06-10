using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Domain
{
    public class Alert : AuditableEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        [Required]
        public int AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft Aircraft { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;
        public DateTime? ResolvedAt { get; set; }
        public List<int> WorkOrderIds { get; set; } = new();
        public bool NotificationSent { get; set; }
    }
}