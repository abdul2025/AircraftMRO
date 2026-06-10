

namespace AircraftMRO.Infrastructure.Identity.Services
{
    public class AuthResult
    {
        public bool Succeeded { get; init; }

        public string? AccessToken { get; init; }

        public string? RefreshToken { get; init; }

        public string? ErrorMessage { get; init; }
    }
}