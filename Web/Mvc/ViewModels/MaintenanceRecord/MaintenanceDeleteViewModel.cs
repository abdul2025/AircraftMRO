using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Mvc.ViewModels.MaintenanceRecord
{
    public class MaintenanceDeleteViewModel
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        public MaintenanceType Type { get; set; }

        public MaintenanceStatus Status { get; set; }
    }
}