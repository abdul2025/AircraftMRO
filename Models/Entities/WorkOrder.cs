using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Models.Entities;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models
{
    public class WorkOrder : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        public int AircraftId { get; set; }

        [ForeignKey(nameof(AircraftId))]
        public Aircraft Aircraft { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

        public DateTime? CompletedAt { get; set; }

        // One WorkOrder -> Many MaintenanceRecords
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }
}