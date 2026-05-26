

using AircraftMRO.Models.ViewModels.Alert;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Models.ViewModels.WorkOrder;

namespace AircraftMRO.Models.ViewModels.Aircraft
{
    public class AircraftDetailsViewModel
    {
         public int Id { get; set; }

        public string TailNumber { get; set; } = string.Empty;

        // Recent Items
        public IEnumerable<MaintenanceLightViewModel> LightMaintenanceRecords { get; set; }
            = new List<MaintenanceLightViewModel>();

        public IEnumerable<WorkOrderLightViewModel> LightWorkOrders { get; set; }
            = new List<WorkOrderLightViewModel>();

        public IEnumerable<AlartLightViewModel> LightAlerts { get; set; }
            = new List<AlartLightViewModel>();
        
    }
}