using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Mvc.ViewModels.Alert
{
    public class AlartLightViewModel
    {
        public int Id { get; set; }
        public AlertSeverity Severity { get; set; }

        public bool IsResolved { get; set; }

    }
}