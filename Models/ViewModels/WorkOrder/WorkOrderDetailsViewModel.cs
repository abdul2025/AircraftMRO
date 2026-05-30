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

    public WorkOrderPriority Priority { get; set; }

    public WorkOrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int MaintenanceRecordCount { get; set; }

    public List<MaintenanceRecordSummaryViewModel> MaintenanceRecords { get; set; }
        = [];
}
}