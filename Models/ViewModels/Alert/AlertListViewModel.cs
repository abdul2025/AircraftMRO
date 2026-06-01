using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.Alert
{
    public class AlertListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string AircraftTailNumber { get; set; } = string.Empty;

        public AlertSeverity Severity { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsResolved { get; set; }

        public bool NotificationSent { get; set; }

        public int? WorkOrderId { get; set; }
    }
}