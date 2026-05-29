using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Models.ViewModels.WorkOrder;

namespace AircraftMRO.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Task<PagedResult<WorkOrderListViewModel>> GetWorkOrdersAsync(WorkOrderFilter filter);
        Task<WorkOrderCreateViewModel> GetCreateViewAsync();
        Task<WorkOrderEditViewModel> GetEdutViewAsync(int id);
    }
}