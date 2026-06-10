using AircraftMRO.Domain.Enums;


namespace AircraftMRO.Mvc.ViewModels.WorkOrder
{
    public class WorkOrderEditViewModel
    {
        public int Id { get; set; }

        public int AircraftId { get; set; }

        public string AircraftTailNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; }

    }
}