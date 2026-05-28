using AircraftMRO.Data;
using AircraftMRO.Models.ViewModels;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using AircraftMRO.Models.ViewModels.Aircraft;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Models.ViewModels.Alert;
using AircraftMRO.Common.Results;
using AircraftMRO.Models;
using AircraftMRO.Repositories;
using Npgsql;
using Npgsql.PostgresTypes;


namespace AircraftMRO.Services
{
    public class AircraftService : IAircraftService
    {

        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _logger;
        private readonly IBaseRepository<Aircraft> _repository;


        public AircraftService(ApplicationDbContext context, IAppLogger logger, IBaseRepository<Aircraft> repository)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
        }



        public async Task<IEnumerable<AircraftListViewModel>> GetAircraftAsync()
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



        public async Task<ServiceResult<AircraftDetailsViewModel>> GetAircraftDetailsAsync(int id)
        {
            try
            {
                AircraftDetailsViewModel? aircraftDetails =
                    await _context.Aircrafts
                        .AsNoTracking() // EF Core does NOT track returned entities in memory
                        .AsSplitQuery() // Splits one massive query into multiple smaller queries
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
                                })
                                .ToList(),

                            LightWorkOrders = a.WorkOrders
                                .OrderByDescending(o => o.Id)
                                .Take(30)
                                .Select(o => new WorkOrderLightViewModel
                                {
                                    Id = o.Id,
                                    Priority = o.Priority,
                                    Status = o.Status
                                })
                                .ToList(),

                            LightAlerts = a.Alerts
                                .OrderByDescending(l => l.Id)
                                .Take(30)
                                .Select(l => new AlartLightViewModel
                                {
                                    Id = l.Id,
                                    Severity = l.Severity,
                                    IsResolved = l.IsResolved
                                })
                                .ToList()
                        })
                        .FirstOrDefaultAsync();

                if (aircraftDetails == null)
                {
                    return new ServiceResult<AircraftDetailsViewModel>
                    {
                        Success = false,
                        ErrorMessage = "Aircraft not found."
                    };
                }

                return new ServiceResult<AircraftDetailsViewModel>
                {
                    Success = true,
                    Data = aircraftDetails
                };
            }
            catch (Exception)
            {
                return new ServiceResult<AircraftDetailsViewModel>
                {
                    Success = false,
                    ErrorMessage = "Failed to load aircraft details."
                };
            }
        }

        public async Task<ServiceResult<Aircraft>> CreateAircraftAsync(AircraftCreateViewModel viewModel)
        {
            try
            {
                Aircraft aircraft = new()
                {
                    TailNumber = viewModel.TailNumber,
                    Model = viewModel.Model.Trim(),
                    Manufacturer = viewModel.Manufacturer.Trim()
                };

                await _repository.AddAsync(aircraft);

                await _repository.SaveChangesAsync();

                return new ServiceResult<Aircraft>
                {
                    Success = true,
                    Data = aircraft
                };
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresEx)
                {
                    if (postgresEx.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        return new ServiceResult<Aircraft>
                        {
                            Success = false,
                            ErrorMessage = "Tail number already exists."
                        };
                    }
                }

                return new ServiceResult<Aircraft>
                {
                    Success = false,
                    ErrorMessage = "Database update failed."
                };
            }
            catch (Exception)
            {

                return new ServiceResult<Aircraft>
                {
                    Success = false,
                    ErrorMessage = "Failed to create aircraft."
                };
            }
        }
    }
}