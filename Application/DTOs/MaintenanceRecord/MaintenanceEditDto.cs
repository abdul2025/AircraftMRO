
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord
{
    public class MaintenanceEditDto
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }
        public MaintenanceType Type { get; set; }
        public MaintenanceStatus Status { get; set; }

        public DateTime ScheduledDate { get; set; }
        public string? Notes { get; set; }

        public List<WorkOrderLookupDto> WorkOrders { get; set; } = [];

    }
}