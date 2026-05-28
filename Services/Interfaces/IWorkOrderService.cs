using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.ViewModels.WorkOrder;

namespace AircraftMRO.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Task<List<WorkOrderListViewModel>>GetWorkOrdersAsync();
    }
}