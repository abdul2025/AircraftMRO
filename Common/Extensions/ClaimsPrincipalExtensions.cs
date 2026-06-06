using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AircraftMRO.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetRoles(this ClaimsPrincipal user)
        {
            return string.Join(
                ", ",
                user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value));
        }
    }
}