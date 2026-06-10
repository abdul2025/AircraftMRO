using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Mvc.ViewModels.WorkOrder
{
    public class WorkOrderLightViewModel
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

    }
}