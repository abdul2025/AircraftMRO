using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain;

namespace AircraftMRO.Services.Interfaces
{
    public interface IAircraftStatusService
    {

        Task UpdateAircraftStatus(Aircraft aircraft, IEnumerable<WorkOrder> workOrders);

    }
}