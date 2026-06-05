using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;

namespace AircraftMRO.Models.ViewModels.WorkOrder
{
    public class WorkOrderDetailsViewModel
    {
        public int Id { get; set; }

        public int AircraftId { get; set; }

        public string AircraftTailNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public WorkOrderPriority Priority { get; set; }

        public WorkOrderStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int MaintenanceRecordCount { get; set; }

        public List<MaintenanceRecordSummaryViewModel> MaintenanceRecords { get; set; } = [];

        public string? CreatedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedAtUtc { get; set; }
    }
}