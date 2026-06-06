using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class AircraftStatusService : IAircraftStatusService
    {
        private readonly IAppLogger<AircraftStatusService> _logger;
        private readonly ApplicationDbContext _context;


        public AircraftStatusService(IAppLogger<AircraftStatusService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        /*
        * Aircraft Status Rules
        *
        * Aircraft status is automatically derived from all Work Orders
        * belonging to the aircraft.
        *
        * Grounded
        *  - At least one Open or InProgress Work Order exists
        *  - AND the Work Order Priority is Critical
        *
        * Maintenance
        *  - At least one Open or InProgress Work Order exists
        *  - AND no Critical Open/InProgress Work Orders exist
        *
        * Active
        *  - No Open or InProgress Work Orders exist
        *  - All Work Orders are Closed
        *
        * Priority Order:
        *
        * Critical Work Order
        *      ↓
        *   Grounded
        *
        * Outstanding Work Order
        *      ↓
        *  Maintenance
        *
        * No Outstanding Work Orders
        *      ↓
        *    Active
        *
        * Examples:
        *
        * Aircraft
        * ├── WO-1 Closed
        * ├── WO-2 Closed
        * └── WO-3 Closed
        *      => Active
        *
        * Aircraft
        * ├── WO-1 Closed
        * ├── WO-2 Open (High)
        * └── WO-3 Closed
        *      => Maintenance
        *
        * Aircraft
        * ├── WO-1 Closed
        * ├── WO-2 InProgress (Critical)
        * └── WO-3 Closed
        *      => Grounded
        *
        * Aircraft
        * ├── WO-1 Closed
        * ├── WO-2 Open (Critical)
        * └── WO-3 Open (Low)
        *      => Grounded
        */
        public void UpdateAircraftStatus(Aircraft aircraft, IEnumerable<WorkOrder> workOrders)
        {
            // Check for Any Open or Inprogress with Critical Priority
            bool hasCritical = workOrders.Any(w =>
                (w.Status == WorkOrderStatus.Open ||
                 w.Status == WorkOrderStatus.InProgress) &&
                w.Priority == WorkOrderPriority.Critical);

            // Check for Any Open or InProgress
            bool hasOutstanding = workOrders.Any(w =>
                w.Status == WorkOrderStatus.Open ||
                w.Status == WorkOrderStatus.InProgress);

            AircraftStatus newStatus = hasCritical ? AircraftStatus.Grounded : hasOutstanding ? AircraftStatus.Maintenance : AircraftStatus.Active;

            _logger.LogInfo("Aircraft status check",
                new
                {
                    AircraftId = aircraft.Id,
                    HasCritical = hasCritical,
                    HasOutstanding = hasOutstanding
                });
            AircraftStatus previousStatus = aircraft.Status;

            if (aircraft.Status != newStatus)
            {
                aircraft.Status = newStatus;

                _logger.LogInfo("Aircraft status recalculated.",
                    new
                    {
                        AircraftId = aircraft.Id,
                        PreviousStatus = previousStatus,
                        NewStatus = newStatus
                    });
            }




            // ******************* ******************* ******************* *******************

            // Resolve active Alerts for Aircraft Back to Active from Grounded. //TODO: THIS SHOULD BE IN ANOTHER SERVICE as Resolve Alerts Service
            if (previousStatus == AircraftStatus.Grounded && newStatus != AircraftStatus.Grounded)
            {
                List<Alert> activeAlerts = _context.Alerts
                    .Where(a =>
                        a.AircraftId == aircraft.Id &&
                        a.ResolvedAt == null &&
                        a.Title == "Aircraft Grounded")
                    .ToList();

                foreach (Alert alert in activeAlerts)
                {
                    alert.ResolvedAt = DateTime.UtcNow;

                    _logger.LogInfo("alert Resolved",
                    new
                    {
                        Alert = alert.Id,
                        AircraftId = alert.AircraftId,
                        AircraftStatus = newStatus
                    });
                }


            }







        }
    }
}