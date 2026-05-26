using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels
{
    public class AircraftListViewModel
    {
        public int Id { get; set; }

        public string TailNumber { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;

        public AircraftStatus Status { get; set; } = AircraftStatus.Active;

        public int TotalFlightHours { get; set; }
        public bool IsDeleted { get; set; }

        public int MaintenanceCount { get; set; }
        public int WorkOrderCount { get; set; }
        public int AlertCount { get; set; }

    }
}