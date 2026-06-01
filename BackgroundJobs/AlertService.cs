using AircraftMRO.Data;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.BackgroundJobs
{
    public class AlertJobsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _logger;

        public AlertJobsService(
            ApplicationDbContext context,
            IAppLogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RunAlertChecks()
        {
            try
            {
                _logger.LogInfo("Starting alert background job.");

                await CheckGroundedAircraftAlerts();
                await CheckOverdueWorkOrdersAlerts();
                await CheckLongMaintenanceAlerts();

                _logger.LogInfo("Alert background job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Alert background job failed.",
                    ex,
                    new { JobName = "AlertBackgroundJob" });
                throw;
            }
        }

        public async Task CheckGroundedAircraftAlerts()
        {
            _logger.LogInfo("Checking grounded aircraft alerts.");

            var groundedAircraft = await _context.Aircrafts
                .Where(a => a.Status == AircraftStatus.Grounded)
                .ToListAsync();

            int createdAlerts = 0;

            foreach (var aircraft in groundedAircraft)
            {
                bool alertExists = await _context.Alerts.AnyAsync(a =>
                    a.AircraftId == aircraft.Id &&
                    !a.ResolvedAt.HasValue &&
                    a.Title == "Aircraft Grounded");

                if (alertExists)
                {
                    _logger.LogInfo(
                        $"Grounded alert already exists for Aircraft {aircraft.Id} ({aircraft.TailNumber}).");

                    continue;
                }

                _context.Alerts.Add(new Alert
                {
                    AircraftId = aircraft.Id,
                    Severity = AlertSeverity.Critical,
                    Title = "Aircraft Grounded",
                    Message = $"Aircraft {aircraft.TailNumber} is currently grounded.",
                    CreatedAt = DateTime.UtcNow
                });

                createdAlerts++;

                _logger.LogWarning(
                    $"Created grounded aircraft alert for Aircraft {aircraft.Id} ({aircraft.TailNumber}).");
            }

            await _context.SaveChangesAsync();

            _logger.LogInfo(
                $"Grounded aircraft alert check completed. Created {createdAlerts} alert(s).");
        }

        public async Task CheckOverdueWorkOrdersAlerts()
        {
            _logger.LogInfo("Checking overdue work order alerts.");

            var threshold = DateTime.UtcNow.AddHours(-24);

            var workOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Where(w =>
                    (w.Priority == WorkOrderPriority.High ||
                     w.Priority == WorkOrderPriority.Critical) &&
                    w.Status != WorkOrderStatus.Closed &&
                    w.CreatedAt <= threshold)
                .ToListAsync();

            int createdAlerts = 0;

            foreach (var workOrder in workOrders)
            {
                bool alertExists = await _context.Alerts.AnyAsync(a =>
                    a.WorkOrderId == workOrder.Id &&
                    !a.ResolvedAt.HasValue &&
                    a.Title == "Overdue Work Order");

                if (alertExists)
                {
                    _logger.LogInfo(
                        $"Overdue alert already exists for WorkOrder {workOrder.Id}.");

                    continue;
                }

                _context.Alerts.Add(new Alert
                {
                    AircraftId = workOrder.AircraftId,
                    WorkOrderId = workOrder.Id,
                    Severity = AlertSeverity.Warning,
                    Title = "Overdue Work Order",
                    Message = $"Work Order #{workOrder.Id} has been open for more than 24 hours.",
                    CreatedAt = DateTime.UtcNow
                });

                createdAlerts++;

                _logger.LogWarning(
                    $"Created overdue work order alert for WorkOrder {workOrder.Id}.");
            }

            await _context.SaveChangesAsync();

            _logger.LogInfo(
                $"Overdue work order alert check completed. Created {createdAlerts} alert(s).");
        }

        public async Task CheckLongMaintenanceAlerts()
        {
            _logger.LogInfo("Checking extended maintenance alerts.");

            var threshold = DateTime.UtcNow.AddDays(-7);

            var aircrafts = await _context.Aircrafts
                .Where(a =>
                    a.Status == AircraftStatus.Maintenance &&
                    a.WorkOrders.Any(w =>
                        w.Status != WorkOrderStatus.Closed &&
                        w.CreatedAt <= threshold))
                .ToListAsync();

            int createdAlerts = 0;

            foreach (var aircraft in aircrafts)
            {
                bool alertExists = await _context.Alerts.AnyAsync(a =>
                    a.AircraftId == aircraft.Id &&
                    !a.ResolvedAt.HasValue &&
                    a.Title == "Extended Maintenance");

                if (alertExists)
                {
                    _logger.LogInfo(
                        $"Extended maintenance alert already exists for Aircraft {aircraft.Id} ({aircraft.TailNumber}).");

                    continue;
                }

                _context.Alerts.Add(new Alert
                {
                    AircraftId = aircraft.Id,
                    Severity = AlertSeverity.Warning,
                    Title = "Extended Maintenance",
                    Message = $"Aircraft {aircraft.TailNumber} has been under maintenance for more than 7 days.",
                    CreatedAt = DateTime.UtcNow
                });

                createdAlerts++;

                _logger.LogWarning(
                    $"Created extended maintenance alert for Aircraft {aircraft.Id} ({aircraft.TailNumber}).");
            }

            await _context.SaveChangesAsync();

            _logger.LogInfo(
                $"Extended maintenance alert check completed. Created {createdAlerts} alert(s).");
        }
    }
}