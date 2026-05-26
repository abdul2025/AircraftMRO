using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.ViewModels.Alert
{
    public class AlartLightViewModel
    {
        public int Id { get; set; }

        public AlertSeverity Severity { get; set; } = AlertSeverity.Warning;
        public bool IsResolved { get; set; } = false;
    }
}