using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.MaintenanceRecord
{
    public class MaintenanceRecordSummaryViewModel
    {
        public int Id { get; set; }

        public MaintenanceType Type { get; set; }

        public MaintenanceStatus Status { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string Notes { get; set; } = string.Empty;
    }
}