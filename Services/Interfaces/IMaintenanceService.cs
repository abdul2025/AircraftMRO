using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Models;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;

namespace AircraftMRO.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<PagedResult<MaintenanceListViewModel>> GetMaintenanceRecordsAsync(MaintenanceFilter filter);
        Task<MaintenanceCreateViewModel> GetCreateViewModelAsync();

        // Create
        Task<MaintenanceCreateViewModel> PopulateCreateViewModelAsync(MaintenanceCreateViewModel viewModel);

        Task<ServiceResult<MaintenanceRecord>> CreateMaintenanceRecordAsync(MaintenanceCreateViewModel viewModel);

        // EDIT
        Task<MaintenanceEditViewModel?> GetEditViewModelAsync(int id);

        Task<MaintenanceEditViewModel> PopulateEditViewModelAsync(MaintenanceEditViewModel viewModel);

        Task<ServiceResult<MaintenanceRecord>> UpdateMaintenanceRecordAsync(MaintenanceEditViewModel viewModel);

        // DEL
        Task<MaintenanceDeleteViewModel?> GetDeleteViewModelAsync(int id);

        Task<ServiceResult<MaintenanceRecord>> DeleteMaintenanceRecordAsync(int id);


        // Complated Acton
        Task<ServiceResult<MaintenanceRecord>> CompleteMaintenanceRecordAsync(int id);

        Task<MaintenanceRecordDetailsViewModel?> GetMaintenanceRecordDetailsAsync(int id);
    }
}