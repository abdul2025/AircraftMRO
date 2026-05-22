using System.ComponentModel.DataAnnotations;
using AircraftMRO.Models.Enums;


namespace AircraftMRO.Models
{
    public class Aircraft{
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TailNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Manufacturer { get; set; } = string.Empty;

        public AircraftStatus Status { get; set; } = AircraftStatus.Active;

        public int TotalFlightHours { get; set; }
        public bool IsDeleted { get; set; }

        // Relationships
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }
}