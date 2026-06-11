

namespace AircraftMRO.Application.DTOs.Aircraft
{
    public class AircraftCreateDto
    {
        public string TailNumber { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;
    }
}