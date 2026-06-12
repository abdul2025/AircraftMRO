
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord
{
    public class MaintenanceListDto
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;

        public MaintenanceType Type { get; set; }
        public MaintenanceStatus Status { get; set; }

        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}