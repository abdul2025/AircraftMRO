using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Application.Events
{
    public class AircraftCreatedEvent: MediatR.INotification
    {
        public int AircraftId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}