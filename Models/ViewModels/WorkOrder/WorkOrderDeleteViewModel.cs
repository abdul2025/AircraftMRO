using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Models.ViewModels.WorkOrder
{
    public class WorkOrderDeleteViewModel
    {
        public int Id { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}