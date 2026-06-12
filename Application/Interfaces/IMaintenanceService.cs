using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Application.DTOs.MaintenanceRecord;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Mvc.ViewModels.MaintenanceRecord;

namespace AircraftMRO.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<PagedResult<MaintenanceListDto>> GetMaintenanceRecordsAsync(MaintenanceFilter filter);

        Task<MaintenanceDetailsDto?> GetMaintenanceRecordDetailsAsync(int id);

        Task<MaintenanceCreateDto> GetCreateAsync();

        Task<MaintenanceCreateDto> PopulateCreateAsync(MaintenanceCreateDto dto);

        Task<ServiceResult<MaintenanceEditDto?>> GetEditAsync(int id);

        Task<MaintenanceEditDto> PopulateEditAsync(MaintenanceEditDto dto);

        Task<ServiceResult<MaintenanceRecord>> CreateAsync(MaintenanceCreateDto dto);

        Task<ServiceResult<MaintenanceRecord>> UpdateAsync(MaintenanceEditDto dto);

        Task<ServiceResult<MaintenanceDeleteDto>> GetDeleteAsync(int id);

        Task<ServiceResult<MaintenanceRecord>> DeleteAsync(int id);

        Task<ServiceResult<MaintenanceRecord>> CompleteAsync(int id);
    }
}