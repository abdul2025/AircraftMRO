using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Infrastructure.Identity.Constants;
using Hangfire.Dashboard;

namespace AircraftMRO.Infrastructure.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            // Console.WriteLine(httpContext.User.IsInRole(Roles.Admin));
            // Console.WriteLine(httpContext.User.Identity.IsAuthenticated);

            return httpContext.User.Identity?.IsAuthenticated == true && httpContext.User.IsInRole(Roles.Admin);
        }
    }
}