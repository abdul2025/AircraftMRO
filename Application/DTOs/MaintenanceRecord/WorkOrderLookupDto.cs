using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Application.DTOs.MaintenanceRecord
{
    public class WorkOrderLookupDto
    {
        public int Id { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }
}