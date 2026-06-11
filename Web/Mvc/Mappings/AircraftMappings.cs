using AircraftMRO.Application.DTOs.Aircraft;
using AircraftMRO.Application.DTOs.WorkOrder;
using AircraftMRO.Application.DTOs.MaintenanceRecord;
using AircraftMRO.Application.DTOs.Alert;

using AircraftMRO.Mvc.ViewModels.Aircraft;
using AircraftMRO.Mvc.ViewModels.WorkOrder;
using AircraftMRO.Mvc.ViewModels.MaintenanceRecord;
using AircraftMRO.Mvc.ViewModels.Alert;

namespace AircraftMRO.Web.Mvc.Mappings
{

    public static class AircraftMappings
    {
        
        public static AircraftDetailsViewModel ToVm(this AircraftDetailsDto dto)
        {
            return new AircraftDetailsViewModel
            {
                Id = dto.Id,
                TailNumber = dto.TailNumber,
                IsDeleted = dto.IsDeleted,

                CreatedAtUtc = dto.CreatedAtUtc,
                CreatedBy = dto.CreatedBy,
                UpdatedAtUtc = dto.UpdatedAtUtc,
                UpdatedBy = dto.UpdatedBy,
                DeletedAtUtc = dto.DeletedAtUtc,
                DeletedBy = dto.DeletedBy,

                LightWorkOrders = dto.LightWorkOrders.Select(x => x.ToVm()).ToList(),
                LightMaintenanceRecords = dto.LightMaintenanceRecords.Select(x => x.ToVm()).ToList(),
                LightAlerts = dto.LightAlerts.Select(x => x.ToVm()).ToList()
            };
        }

        // =========================================================
        // LIGHT WORK ORDERS
        // =========================================================
        public static WorkOrderLightViewModel ToVm(this WorkOrderLightDto dto)
        {
            return new WorkOrderLightViewModel
            {
                Id = dto.Id,
                Priority = dto.Priority,
                Status = dto.Status,
                IsDeleted = dto.IsDeleted
            };
        }

        // =========================================================
        // LIGHT MAINTENANCE RECORDS
        // =========================================================
        public static MaintenanceLightViewModel ToVm(this MaintenanceLightDto dto)
        {
            return new MaintenanceLightViewModel
            {
                Id = dto.Id,
                WorkOrderId = dto.WorkOrderId,
                Type = dto.Type,
                Status = dto.Status,
                IsDeleted = dto.IsDeleted
            };
        }

        // =========================================================
        // LIGHT ALERTS
        // =========================================================
        public static AlartLightViewModel ToVm(this AlertLightDto dto)
        {
            return new AlartLightViewModel
            {
                Id = dto.Id,
                Severity = dto.Severity,
                IsResolved = dto.IsResolved
            };
        }
    }
}