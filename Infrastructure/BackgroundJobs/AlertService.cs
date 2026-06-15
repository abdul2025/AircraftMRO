using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Domain;
using AircraftMRO.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;
using AircraftMRO.Application.Events;
using MediatR;

namespace AircraftMRO.Infrastructure.BackgroundJobs
{
    public class AlertJobsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<AlertJobsService> _logger;
        private readonly IMediator _mediator;

        public AlertJobsService(ApplicationDbContext context, IAppLogger<AlertJobsService> logger, IMediator mediator)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task RunAlertChecks()
        {
            try
            {
                _logger.LogInfo("Starting alert background job.");

                await CheckOverdueWorkOrdersAlerts();

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


        public async Task CheckOverdueWorkOrdersAlerts()
        {
            _logger.LogInfo("Checking overdue work order alerts.");

            var threshold = DateTime.UtcNow.AddHours(-24);

            var overdueWorkOrders = await _context.WorkOrders
                .Include(w => w.Aircraft)
                .Where(w =>
                   (
                        w.Priority == WorkOrderPriority.High || w.Priority == WorkOrderPriority.Critical) &&
                        w.Status != WorkOrderStatus.Closed &&
                        w.CreatedAtUtc <= threshold
                    )
                .ToListAsync();

            int triggeredEvents = 0;

            foreach (var wo in overdueWorkOrders)
            {
                // Check if an active "Overdue" alert already exists for this specific work order
                // We use DataPayload or a custom field to link, here we check by existing notification message
                bool exists = await _context.Notifications.AnyAsync(n => n.Type == NotificationType.OverdueWorkOrder
                                && n.ResolvedAt == null
                                && n.DataPayload!.Contains(wo.Id.ToString()));
                
                if (!exists)
                {
                    await _mediator.Publish(new WorkOrderOverdueEvent
                    {
                        WorkOrderId = wo.Id,
                        AircraftId = wo.AircraftId,
                        TailNumber = wo.Aircraft.TailNumber,
                        Description = wo.Description
                    });

                    triggeredEvents++;
                    _logger.LogWarning($"Triggered WorkOrderOverdueEvent for WorkOrder {wo.Id}.");
                }
            }

            _logger.LogInfo($"Overdue work order alert check completed. Triggered {triggeredEvents} event(s).");
        }
    }
}