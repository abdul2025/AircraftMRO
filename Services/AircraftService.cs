using AircraftMRO.Data;
using AircraftMRO.Models.ViewModels;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using AircraftMRO.Models.ViewModels.Aircraft;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Models.ViewModels.Alert;


namespace AircraftMRO.Services
{
    public class AircraftService : IAircraftService
    {

        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _logger;

        public AircraftService(ApplicationDbContext context, IAppLogger logger)
        {
            _context = context;
            _logger = logger;
        }



        public async Task<IEnumerable<AircraftListViewModel>> AsyncGetAircraft()
        {
            var aircrafts = await _context.Aircrafts.Select(a => new AircraftListViewModel
            {
                Id = a.Id,
                TailNumber = a.TailNumber,
                Model = a.Model,
                Manufacturer = a.Manufacturer,
                Status = a.Status,
                TotalFlightHours = a.TotalFlightHours,
                MaintenanceCount = a.MaintenanceRecords.Count(),
                WorkOrderCount = a.WorkOrders.Count(),
                AlertCount = a.Alerts.Count(),

            }).ToListAsync();

            return aircrafts;
        }



        public async Task<AircraftDetailsViewModel?> AsyncGetAircraftDetails(int id)
        {
            var aircraftDetails = await _context.Aircrafts
                .Where(a => a.Id == id)
                .Select(a => new AircraftDetailsViewModel
                {
                    Id = a.Id,
                    TailNumber = a.TailNumber,
                    LightMaintenanceRecords = a.MaintenanceRecords
                        .OrderByDescending(m => m.Id)
                        .Take(30)
                        .Select(m => new MaintenanceLightViewModel
                        {
                            Id = m.Id,
                            Type = m.Type,
                            Status = m.Status
                        }),

                    LightWorkOrders = a.WorkOrders
                        .OrderByDescending(o => o.Id)
                        .Take(30)
                        .Select(o => new WorkOrderLightViewModel
                        {
                            Id = o.Id,
                            Priority = o.Priority,
                            Status = o.Status
                        }),

                    LightAlerts = a.Alerts
                        .OrderByDescending(l => l.Id)
                        .Take(30)
                        .Select(l => new AlartLightViewModel
                        {
                            Id = l.Id,
                            Severity = l.Severity,
                            IsResolved = l.IsResolved
                        })
                })
                .FirstOrDefaultAsync();

            return aircraftDetails;
        }

    }
}