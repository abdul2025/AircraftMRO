using AircraftMRO.Application.DTOs.Alert;
using AircraftMRO.Common.Pagination;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAlertService
    {
        Task<PagedResult<AlertListDto>> GetAlertsAsync(string? search, bool? resolved, int pageNumber, int pageSize);
    }
}
