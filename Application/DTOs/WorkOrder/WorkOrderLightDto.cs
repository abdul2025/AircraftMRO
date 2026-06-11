using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.WorkOrder
{
    public class WorkOrderLightDto
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
    }
}