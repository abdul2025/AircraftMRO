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
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Filters;


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



        public async Task<PagedResult<AircraftListViewModel>> GetAircraftAsync(AircraftFilter filter)
        {
            // IQueryable Making it as query dynamically with filter and where, and happen in db not in memory  
            IQueryable<Aircraft> query = _context.Aircrafts.AsNoTracking(); // Read no need for tracking of changes, less memory usage and faster queries 

            // both above and below doing the same but above is more explicit telling var query is IQueryable
            // var query = _context.Aircrafts
            // .AsNoTracking() 
            // .AsQueryable(); 
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();
                // Search by text input
                query = query.Where(a => // EF.Functions.ILike PostgreSQL's and Case-insensitive
                    EF.Functions.ILike(a.TailNumber, $"%{search}%") ||
                    EF.Functions.ILike(a.Model, $"%{search}%") ||
                    EF.Functions.ILike(a.Manufacturer, $"%{search}%"));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(a => new AircraftListViewModel
                {
                    Id = a.Id,
                    TailNumber = a.TailNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    Status = a.Status,
                    TotalFlightHours = a.TotalFlightHours,

                    MaintenanceCount = a.WorkOrders
                        .SelectMany(w => w.MaintenanceRecords)
                        .Count(),

                    WorkOrderCount = a.WorkOrders.Count(),

                    AlertCount = a.Alerts.Count(),

                    IsDeleted = a.IsDeleted
                })
                .ToListAsync();

            return new PagedResult<AircraftListViewModel>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }



        public async Task<ServiceResult<AircraftDetailsViewModel>> GetAircraftDetailsAsync(int id)
        {
            try
            {
                AircraftDetailsViewModel? aircraftDetails =
                    await _context.Aircrafts
                        .AsNoTracking() // Read no need for tracking of changes, less memory usage and faster queries 
                        .Where(a => a.Id == id)
                        .Select(a => new AircraftDetailsViewModel
                        {
                            Id = a.Id,

                            TailNumber = a.TailNumber,

                            LightMaintenanceRecords = a.WorkOrders
                                .SelectMany(w => w.MaintenanceRecords)
                                .OrderByDescending(m => m.Id)
                                .Take(30)
                                .Select(m => new MaintenanceLightViewModel
                                {
                                    Id = m.Id,
                                    WorkOrderId = m.WorkOrderId,
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
                                .ToList(),
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