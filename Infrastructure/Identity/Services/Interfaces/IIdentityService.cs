

using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Models;

namespace AircraftMRO.Infrastructure.Identity.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResult> AuthenticateAsync(string email, string password);
        Task<ServiceResult<CreateUserResult>> CreateUserAsync(CreateUserRequest request);
        Task LogoutAsync();
    }
}