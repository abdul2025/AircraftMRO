

namespace AircraftMRO.Application.DTOs.EmailTemplates
{
    public class OverdueWorkOrderEmailModel
    {
        public int WorkOrderId { get; set; }
        public int AircraftId { get; set; }
        public string Url { get; set; } = string.Empty;
        // Fields below are optional if they aren't in the JSON payload
        public string? TailNumber { get; set; }
        public string? Description { get; set; }
    }
}