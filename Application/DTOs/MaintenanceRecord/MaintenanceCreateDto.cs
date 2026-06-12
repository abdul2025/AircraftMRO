
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord
{
    public class MaintenanceCreateDto
    {
        public int WorkOrderId { get; set; }
        public MaintenanceType Type { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? Notes { get; set; }

        // dropdown source
        public List<WorkOrderLookupDto> WorkOrders { get; set; } = [];
    }
}