using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Application.Events
{
    public class AircraftGroundedEvent: MediatR.INotification
    {
        public int AircraftId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}