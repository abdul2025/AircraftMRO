
namespace AircraftMRO.Application.DTOs.EmailTemplates
{
    public class GroundedAircraftModel
    {
        public int AircraftId { get; set; }
        public string Status { get; set; } = string.Empty; // Added to match your handler
        public string Url { get; set; } = string.Empty;
        // Fields below are optional if they aren't in the JSON payload
        public string? TailNumber { get; set; }
        public DateTime? GroundedAt { get; set; }
    }
}