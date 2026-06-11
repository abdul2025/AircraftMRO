
namespace AircraftMRO.Application.DTOs.Aircraft
{
    public class AircraftEditDto
    {
        public int Id { get; set; }

        public string TailNumber { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;
    }
}