using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Mvc.ViewModels.Aircraft;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using AircraftMRO.Mvc.ViewModels.MaintenanceRecord;
using AircraftMRO.Mvc.ViewModels.WorkOrder;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Repositories;
using Npgsql;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Filters;
using AircraftMRO.Mvc.ViewModels.Alert;


namespace AircraftMRO.Services
{
    public class AircraftService : IAircraftService
    {

        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<AircraftService> _logger;
        private readonly IBaseRepository<Aircraft> _repository;


        public AircraftService(ApplicationDbContext context, IAppLogger<AircraftService> logger, IBaseRepository<Aircraft> repository)
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

                bool isIdSearch = int.TryParse(search, out int aircraftId);

                query = query.Where(a =>
                    (isIdSearch && a.Id == aircraftId) ||
                    EF.Functions.ILike(a.TailNumber, $"%{search}%") ||
                    EF.Functions.ILike(a.Model, $"%{search}%") ||
                    EF.Functions.ILike(a.Manufacturer, $"%{search}%"));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize) // ignore the first X
                .Take(filter.PageSize) // Getting only X records
                .Select(a => new AircraftListViewModel
                {
                    Id = a.Id,
                    TailNumber = a.TailNumber,
                    Model = a.Model,
                    Manufacturer = a.Manufacturer,
                    Status = a.Status,
                    TotalFlightHours = a.TotalFlightHours,

                    MaintenanceCount = a.WorkOrders
                        .SelectMany(w => w.MaintenanceRecords) // SelectMany Flat result of the query, all items in on list
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
                            IsDeleted = a.IsDeleted,
                            CreatedAtUtc = a.CreatedAtUtc,
                            CreatedBy = a.CreatedByUser != null ? a.CreatedByUser.FullName : null,
                            UpdatedAtUtc = a.UpdatedAtUtc,
                            UpdatedBy = a.UpdatedByUser != null ? a.UpdatedByUser.FullName : null,
                            DeletedAtUtc = a.DeletedAtUtc,
                            DeletedBy = a.DeletedByUser != null ? a.DeletedByUser.FullName : null,
                            // Collection 
                            LightWorkOrders = a.WorkOrders // TODO : Add a further condition to get accurate required WorkOrders 
                                .OrderByDescending(o => o.Id)
                                .Take(30)
                                .Select(o => new WorkOrderLightViewModel
                                {
                                    Id = o.Id,
                                    Priority = o.Priority,
                                    Status = o.Status,
                                    IsDeleted = o.IsDeleted
                                })
                                .ToList(),
                            // Collection 
                            LightMaintenanceRecords = a.WorkOrders // TODO : Add a further condition to get accurate required MaintenanceRecords 
                                .SelectMany(w => w.MaintenanceRecords)
                                .OrderByDescending(m => m.Id)
                                .Take(30)
                                .Select(m => new MaintenanceLightViewModel
                                {
                                    Id = m.Id,
                                    WorkOrderId = m.WorkOrderId,
                                    Type = m.Type,
                                    Status = m.Status,
                                    IsDeleted = m.IsDeleted
                                })
                                .ToList(),
                            // Collection 
                            LightAlerts = a.Alerts
                                .OrderByDescending(l => l.Id)
                                .Take(30)
                                .Select(l => new AlartLightViewModel
                                {
                                    Id = l.Id,
                                    Severity = l.Severity,
                                    IsResolved = l.ResolvedAt.HasValue
                                })
                                .ToList(),


                        })
                        .FirstOrDefaultAsync(); // First or Null 

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
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to load aircraft details.",
                    ex,
                    new { AircraftId = id });

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

                _logger.LogInfo(
                    "Aircraft created successfully.",
                    new
                    {
                        aircraft.Id,
                        aircraft.TailNumber
                    });

                return new ServiceResult<Aircraft>
                {
                    Success = true,
                    Data = aircraft
                };
            }
            // Db Speicify 
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresEx)
                {
                    if (postgresEx.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        _logger.LogWarning(
                            "Attempted to create aircraft with duplicate tail number.",
                            new
                            {
                                viewModel.TailNumber
                            });

                        return new ServiceResult<Aircraft>
                        {
                            Success = false,
                            ErrorMessage = "Tail number already exists."
                        };
                    }
                }

                _logger.LogError(
                    "Database update failed while creating aircraft.",
                    ex,
                    new
                    {
                        viewModel.TailNumber,
                        viewModel.Model,
                        viewModel.Manufacturer
                    });

                return new ServiceResult<Aircraft>
                {
                    Success = false,
                    ErrorMessage = "Database update failed."
                };
            }
            // General Catch
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unexpected error while creating aircraft.",
                    ex,
                    new
                    {
                        viewModel.TailNumber,
                        viewModel.Model,
                        viewModel.Manufacturer
                    });

                return new ServiceResult<Aircraft>
                {
                    Success = false,
                    ErrorMessage = "Failed to create aircraft."
                };
            }
        }
    }
}