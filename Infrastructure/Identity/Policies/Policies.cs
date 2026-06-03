using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Infrastructure.Identity.Policies
{
    public class Policies
    {

    public const string ManageAircraft =
        nameof(ManageAircraft);

    public const string ManageWorkOrders =
        nameof(ManageWorkOrders);

    public const string ViewDashboard =
        nameof(ViewDashboard);
        
    }
}