using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord
{
    public class MaintenanceDeleteDto
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public MaintenanceType Type { get; set; }
        public MaintenanceStatus Status { get; set; }

    }
}