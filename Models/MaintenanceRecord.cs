using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }

        [Required]
        public int AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft Aircraft { get; set; } = null!;

        public MaintenanceType Type { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        [MaxLength(500)]
        public string? Notes { get; set; }
}
}