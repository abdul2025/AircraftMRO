using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Models.ViewModels.MaintenanceRecord
{
    public class MaintenanceRecordDetailsViewModel
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        public string WorkOrderDescription { get; set; } = string.Empty;

        public MaintenanceType Type { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public MaintenanceStatus Status { get; set; }

        public string? Notes { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
        public string? DeletedBy { get; set; }

        public DateTime? DeletedAtUtc { get; set; }
    }
}