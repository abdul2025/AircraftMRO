using AircraftMRO.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AircraftMRO.Mvc.ViewModels.WorkOrder
{
    public class WorkOrderListViewModel
    {
        public int Id { get; set; }

        [Required]
        public int AircraftId { get; set; }

        public string AircraftTailNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; } 

        public int MaintenanceRecordCount { get; set; }

    }
}