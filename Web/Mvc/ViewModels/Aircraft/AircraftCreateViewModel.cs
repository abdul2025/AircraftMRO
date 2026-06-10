using System.ComponentModel.DataAnnotations;

namespace AircraftMRO.Mvc.ViewModels.Aircraft
{
    public class AircraftCreateViewModel
    {
        [Required]
        public string TailNumber { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public string Manufacturer { get; set; } = string.Empty;
    }
}