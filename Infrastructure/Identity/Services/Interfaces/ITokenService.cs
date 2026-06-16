
using AircraftMRO.Infrastructure.Identity.Entities;

namespace AircraftMRO.Infrastructure.Identity.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}