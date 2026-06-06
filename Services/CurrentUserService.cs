using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AircraftMRO.Services.Interfaces;

namespace AircraftMRO.Services
{
    // This service to be used in the ApplicationDBContext to overwrite SaveAsync for auditEntity where UserID required, for any save to DB

    public class CurrentUserService : ICurrentUserService
    {

        private readonly IHttpContextAccessor _httpContextAccessor; // allows classes outside Controllers to access the current request

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }



        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}