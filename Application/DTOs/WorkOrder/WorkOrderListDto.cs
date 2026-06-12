
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.DTOs.WorkOrder
{
    public class WorkOrderListDto
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; }
        public WorkOrderStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int MaintenanceRecordCount { get; set; }
    }

    public class WorkOrderDetailsDto
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; }
        public WorkOrderStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public int MaintenanceRecordCount { get; set; }
        public List<MaintenanceRecordSummaryDto> MaintenanceRecords { get; set; } = new();
    }

    public class MaintenanceRecordSummaryDto
    {
        public int Id { get; set; }
        public MaintenanceType Type { get; set; }
        public MaintenanceStatus Status { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class WorkOrderCreateDto
    {
        public int AircraftId { get; set; }
        public string Description { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; }
        public List<AircraftLookupDto> Aircrafts { get; set; } = [];
    }

    public class WorkOrderEditDto
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkOrderPriority Priority { get; set; }
        public List<AircraftLookupDto> Aircrafts { get; set; } = [];
    }

    public class WorkOrderDeleteDto
    {
        public int Id { get; set; }
        public string AircraftTailNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class WorkOrderLightDto
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;
    }

    public class AircraftLookupDto
    {
        public int Id { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }
}