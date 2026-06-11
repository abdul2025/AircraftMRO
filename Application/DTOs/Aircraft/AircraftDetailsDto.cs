
using AircraftMRO.Application.DTOs.Alert;
using AircraftMRO.Application.DTOs.MaintenanceRecord;
using AircraftMRO.Application.DTOs.WorkOrder;

namespace AircraftMRO.Application.DTOs.Aircraft
{
    public class AircraftDetailsDto
    {
        public int Id { get; set; }

        public string TailNumber { get; set; } = string.Empty;

        public IEnumerable<MaintenanceLightDto> LightMaintenanceRecords { get; set; } = [];

        public IEnumerable<WorkOrderLightDto> LightWorkOrders { get; set; } = [];

        public IEnumerable<AlertLightDto> LightAlerts { get; set; } = [];

        public bool IsDeleted { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? DeletedAtUtc { get; set; }

        public string? DeletedBy { get; set; }
    }
}