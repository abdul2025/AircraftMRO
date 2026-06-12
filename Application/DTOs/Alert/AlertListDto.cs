using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.Alert
{
    public class AlertListDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string AircraftTailNumber { get; set; } = string.Empty;

        public AlertSeverity Severity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public bool IsResolved { get; set; }

        public bool NotificationSent { get; set; }

        public List<int>? WorkOrderIds { get; set; }
    }
}
