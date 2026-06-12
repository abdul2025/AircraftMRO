using AircraftMRO.Application.DTOs.WorkOrder;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;

namespace AircraftMRO.Services.Interfaces
{
    public interface IWorkOrderService
    {
        Task<PagedResult<WorkOrderListDto>> GetWorkOrdersAsync(WorkOrderFilter filter);
        Task<WorkOrderCreateDto> GetCreateDtoAsync();
        Task<ServiceResult<WorkOrder>> CreateAsync(WorkOrderCreateDto dto);
        Task<ServiceResult<WorkOrderDetailsDto>> GetDetailsAsync(int id);
        Task<ServiceResult<WorkOrderEditDto>> GetEditAsync(int id);
        Task<WorkOrderEditDto> PopulateEditAsync(WorkOrderEditDto dto);
        Task<ServiceResult<WorkOrder>> EditAsync(WorkOrderEditDto dto);
        Task<ServiceResult<WorkOrderDeleteDto>> GetDeleteAsync(int id);
        Task<ServiceResult<WorkOrder>> DeleteAsync(int id);

    }
}