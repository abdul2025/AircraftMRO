using System.ComponentModel.DataAnnotations;
using AircraftMRO.Models.Entities;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models
{
    public class Aircraft : AuditableEntity
    {
        public int Id { get; set; }

        private string _tailNumber = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TailNumber
        {
            get => _tailNumber;
            set => _tailNumber = value.Trim().ToUpper();
        }

        [Required]
        [MaxLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Manufacturer { get; set; } = string.Empty;

        public AircraftStatus Status { get; set; } = AircraftStatus.Active;

        public int TotalFlightHours { get; set; }


        // Relationships
        public ICollection<WorkOrder> WorkOrders { get; set; }
            = new List<WorkOrder>();

        public ICollection<Alert> Alerts { get; set; }
            = new List<Alert>();
    }
}