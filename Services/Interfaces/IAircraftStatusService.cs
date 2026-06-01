using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAircraftStatusService
    {
        void UpdateAircraftStatus(Aircraft aircraft, IEnumerable<WorkOrder> workOrders);

    }
}