using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Infrastructure.Identity.Constants
{
    public static class Roles
    {
        public const string Admin = nameof(Admin);

        public const string MaintenanceManager = nameof(MaintenanceManager);

        public const string Engineer = nameof(Engineer);

        public const string Viewer = nameof(Viewer);

        public static readonly string[] All =
        {
            Admin,
            MaintenanceManager,
            Engineer,
            Viewer
        };
    }
}