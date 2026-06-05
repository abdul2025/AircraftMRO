using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.MaintenanceRecord
{
    public class MaintenanceLightViewModel
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public bool IsDeleted { get; set; }

        public MaintenanceType Type { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;


    }
}