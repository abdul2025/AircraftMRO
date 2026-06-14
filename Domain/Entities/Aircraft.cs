using System.ComponentModel.DataAnnotations;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Domain
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
        // One Aircraft to many WorkOrders
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        
        // One Aircraft to many Notifications
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}