using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class AircraftStatusService : IAircraftStatusService
    {
        private readonly IAppLogger _logger;

        public AircraftStatusService(IAppLogger logger)
        {
            _logger = logger;
        }

        public void UpdateAircraftStatus(Aircraft aircraft, IEnumerable<WorkOrder> workOrders)
        {
            bool hasCritical = workOrders.Any(w =>
                (w.Status == WorkOrderStatus.Open ||
                 w.Status == WorkOrderStatus.InProgress) &&
                w.Priority == WorkOrderPriority.Critical);

            bool hasOutstanding = workOrders.Any(w =>
                w.Status == WorkOrderStatus.Open ||
                w.Status == WorkOrderStatus.InProgress);

            AircraftStatus newStatus =
                hasCritical
                    ? AircraftStatus.Grounded
                    : hasOutstanding
                        ? AircraftStatus.Maintenance
                        : AircraftStatus.Active;

            _logger.LogInfo(
                "Aircraft status check",
                new
                {
                    AircraftId = aircraft.Id,
                    HasCritical = hasCritical,
                    HasOutstanding = hasOutstanding
                });

            if (aircraft.Status != newStatus)
            {
                AircraftStatus previousStatus = aircraft.Status;

                aircraft.Status = newStatus;

                _logger.LogInfo(
                    "Aircraft status recalculated.",
                    new
                    {
                        AircraftId = aircraft.Id,
                        PreviousStatus = previousStatus,
                        NewStatus = newStatus
                    });
            }
        }
    }
}