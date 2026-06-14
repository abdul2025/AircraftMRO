
namespace AircraftMRO.Application.Events
{
    public class WorkOrderOverdueEvent : MediatR.INotification
    {
        public int WorkOrderId { get; set; }
        public int AircraftId { get; set; }
        public string TailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}