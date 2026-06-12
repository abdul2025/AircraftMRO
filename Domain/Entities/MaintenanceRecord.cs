using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Domain
{
    public class MaintenanceRecord : AuditableEntity
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        [ForeignKey(nameof(WorkOrderId))]
        public WorkOrder WorkOrder { get; set; } = null!;

        public MaintenanceType Type { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}