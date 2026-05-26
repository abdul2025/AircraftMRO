using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models
{
    public class Alert
    {
        public int Id { get; set; }

        [Required]
        public int AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft Aircraft { get; set; } = null!;

        [Required]
        [MaxLength(300)]
        public string Message { get; set; } = string.Empty;

        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;
    }
}