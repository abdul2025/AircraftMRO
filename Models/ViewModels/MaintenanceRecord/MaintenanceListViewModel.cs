using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.MaintenanceRecord
{
    public class MaintenanceListViewModel
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        public string AircraftTailNumber { get; set; } = string.Empty;

        public MaintenanceType Type { get; set; }

        public MaintenanceStatus Status { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}