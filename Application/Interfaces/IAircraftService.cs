
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Mvc.ViewModels.Aircraft;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAircraftService
    {
        Task<PagedResult<AircraftListViewModel>> GetAircraftAsync(AircraftFilter filter);
        Task<ServiceResult<AircraftDetailsViewModel>> GetAircraftDetailsAsync(int id);
        Task<ServiceResult<Aircraft>> CreateAircraftAsync(AircraftCreateViewModel viewModel);

    }
}