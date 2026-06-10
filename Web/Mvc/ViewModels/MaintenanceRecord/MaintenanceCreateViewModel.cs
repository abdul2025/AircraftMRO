
using AircraftMRO.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AircraftMRO.Mvc.ViewModels.MaintenanceRecord
{
    public class MaintenanceCreateViewModel
    {
        public int AircraftId { get; set; }

        public int WorkOrderId { get; set; }

        public MaintenanceType Type { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string? Notes { get; set; }


        public IEnumerable<SelectListItem> WorkOrders { get; set; } = [];
    }
}