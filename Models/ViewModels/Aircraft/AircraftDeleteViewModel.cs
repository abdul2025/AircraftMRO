using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Models.ViewModels.Aircraft
{
    public class AircraftDeleteViewModel
    {
        public int Id { get; set; }

        public string TailNumber { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
    }
}