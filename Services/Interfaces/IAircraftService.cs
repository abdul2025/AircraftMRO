
using AircraftMRO.Models.ViewModels;
using AircraftMRO.Models.ViewModels.Aircraft;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAircraftService
    {
        Task<IEnumerable<AircraftListViewModel>> AsyncGetAircraft();
        Task<AircraftDetailsViewModel?> AsyncGetAircraftDetails(int id);
        Task<AircraftCreateViewModel> CreateAsync(AircraftCreateViewModel model);
    }
}