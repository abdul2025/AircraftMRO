using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Models.ViewModels.WorkOrder;

namespace AircraftMRO.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Task<PagedResult<WorkOrderListViewModel>> GetWorkOrdersAsync(WorkOrderFilter filter);
        Task<WorkOrderCreateViewModel> GetCreateViewAsync();
        Task<ServiceResult<WorkOrderDetailsViewModel>> GetDetailsAsync(int id);
        Task<ServiceResult<WorkOrderEditViewModel>> GetEditViewAsync(int id);
        Task<ServiceResult<WorkOrderDeleteViewModel>> GetDeleteViewAsync(int id);
        
    }
}