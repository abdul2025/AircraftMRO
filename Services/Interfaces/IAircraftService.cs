
using AircraftMRO.Common.Results;
using AircraftMRO.Models;
using AircraftMRO.Models.ViewModels;
using AircraftMRO.Models.ViewModels.Aircraft;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAircraftService
    {
        Task<IEnumerable<AircraftListViewModel>> GetAircraftAsync();
        Task<ServiceResult<AircraftDetailsViewModel>> GetAircraftDetailsAsync(int id);
        Task<ServiceResult<Aircraft>> CreateAircraftAsync(AircraftCreateViewModel viewModel);

    }
}