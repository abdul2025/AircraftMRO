using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Models.ViewModels.Aircraft
{
    public class AircraftEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string TailNumber { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string Manufacturer { get; set; } = string.Empty;
        
    }
}