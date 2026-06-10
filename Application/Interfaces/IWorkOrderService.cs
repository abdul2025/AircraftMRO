using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Mvc.ViewModels.WorkOrder;

namespace AircraftMRO.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Task<PagedResult<WorkOrderListViewModel>> GetWorkOrdersAsync(WorkOrderFilter filter);
        Task<WorkOrderCreateViewModel> GetCreateViewAsync();
        Task<ServiceResult<WorkOrder>> CreateAsync(WorkOrderCreateViewModel model);
        Task<ServiceResult<WorkOrderDetailsViewModel>> GetDetailsAsync(int id);
        Task<ServiceResult<WorkOrderEditViewModel>> GetEditViewAsync(int id);
        Task<ServiceResult<WorkOrder>> EditAsync(WorkOrderEditViewModel model);
        Task<ServiceResult<WorkOrderDeleteViewModel>> GetDeleteViewAsync(int id);
        Task<ServiceResult<WorkOrder>> DeleteAsync(int id);
        
    }
}