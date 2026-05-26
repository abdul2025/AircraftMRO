using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.WorkOrder
{
    public class WorkOrderLightViewModel
    {
        public int Id { get; set; }
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

    }
}