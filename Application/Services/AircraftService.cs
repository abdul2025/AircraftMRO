using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Repositories;
using Npgsql;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Filters;
using AircraftMRO.Application.DTOs.Aircraft;
using AircraftMRO.Application.DTOs.Alert;
using AircraftMRO.Application.DTOs.MaintenanceRecord;
using AircraftMRO.Application.DTOs.WorkOrder;


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



        public async Task<PagedResult<AircraftListDto>> GetAircraftAsync(AircraftFilter filter)
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
                .Select(a => new AircraftListDto
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

            return new PagedResult<AircraftListDto>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }



        public async Task<ServiceResult<AircraftDetailsDto>> GetAircraftDetailsAsync(int id)
        {
            try
            {
                AircraftDetailsDto? aircraftDetails =
                    await _context.Aircrafts
                        .AsNoTracking() // Read no need for tracking of changes, less memory usage and faster queries 
                        .Where(a => a.Id == id)
                        .Select(a => new AircraftDetailsDto
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
                                .Select(o => new WorkOrderLightDto
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
                                .Select(m => new MaintenanceLightDto
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
                                .Select(l => new AlertLightDto
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
                    return new ServiceResult<AircraftDetailsDto>
                    {
                        Success = false,
                        ErrorMessage = "Aircraft not found."
                    };
                }

                return new ServiceResult<AircraftDetailsDto>
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

                return new ServiceResult<AircraftDetailsDto>
                {
                    Success = false,
                    ErrorMessage = "Failed to load aircraft details."
                };
            }
        }

        public async Task<ServiceResult<Aircraft>> CreateAircraftAsync(AircraftCreateDto dto)
        {
            try
            {
                // 1. Sanitize the data
                dto.TailNumber = dto.TailNumber.Trim();
                dto.Model = dto.Model.Trim();
                dto.Manufacturer = dto.Manufacturer.Trim();


                // 2. Map and Save
                Aircraft aircraft = new()
                {
                    TailNumber = dto.TailNumber,
                    Model = dto.Model,
                    Manufacturer = dto.Manufacturer
                };

                await _repository.AddAsync(aircraft);
                await _repository.SaveChangesAsync();

                return ServiceResult<Aircraft>.SuccessResult(aircraft);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException postgresEx &&
                    postgresEx.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    _logger.LogError("Attempted to create aircraft with duplicate tail number.", ex, new { dto.TailNumber });
                    return ServiceResult<Aircraft>.Failure("Tail number already exists.");
                }

                _logger.LogError("Database update failed while creating aircraft.", ex, new { dto.TailNumber, dto.Model, dto.Manufacturer });
                return ServiceResult<Aircraft>.Failure("Database update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while creating aircraft.", ex, new { dto.TailNumber, dto.Model, dto.Manufacturer });
                return ServiceResult<Aircraft>.Failure("Failed to create aircraft.");
            }
        }

        public async Task<ServiceResult<AircraftEditDto>> GetForEditAsync(int id)
        {
            var aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return new ServiceResult<AircraftEditDto>
                {
                    Success = false,
                    ErrorMessage = "Not found"
                };
            }

            return new ServiceResult<AircraftEditDto>
            {
                Success = true,
                Data = new AircraftEditDto
                {
                    Id = aircraft.Id,
                    TailNumber = aircraft.TailNumber,
                    Model = aircraft.Model,
                    Manufacturer = aircraft.Manufacturer
                }
            };
        }

        public async Task<ServiceResult<Aircraft>> UpdateAsync(AircraftEditDto dto)
        {
            try
            {
                // 1. Sanitize the data
                dto.Model = dto.Model.Trim();
                dto.Manufacturer = dto.Manufacturer.Trim();

                // 2. Fetch the entity
                var aircraft = await _repository.GetByIdAsync(dto.Id);

                if (aircraft == null)
                    return ServiceResult<Aircraft>.Failure("Aircraft not found.");

                // 3. Map and Save
                aircraft.Model = dto.Model;
                aircraft.Manufacturer = dto.Manufacturer;

                await _repository.SaveChangesAsync();

                return ServiceResult<Aircraft>.SuccessResult(aircraft);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unexpected error while updating aircraft.",
                    ex,
                    new
                    {
                        dto.Id,
                        dto.Model,
                        dto.Manufacturer
                    });

                return ServiceResult<Aircraft>.Failure("Failed to update aircraft.");
            }
        }

        public async Task<ServiceResult<AircraftDeleteDto>> GetForDeleteAsync(int id)
        {
            var aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return new ServiceResult<AircraftDeleteDto>
                {
                    Success = false,
                    ErrorMessage = "Not found"
                };
            }

            return new ServiceResult<AircraftDeleteDto>
            {
                Success = true,
                Data = new AircraftDeleteDto
                {
                    Id = aircraft.Id,
                    TailNumber = aircraft.TailNumber,
                    Model = aircraft.Model
                }
            };
        }


        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {


            var aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {

                return new ServiceResult<bool>
                {
                    Success = false,
                    ErrorMessage = "Not found"
                };
            }

            await _repository.DeleteAsync(aircraft);
            await _repository.SaveChangesAsync();

            return new ServiceResult<bool> { Success = true };
        }


        // ERROR MAPPING HELPER METHOD
        private static Dictionary<string, string[]> MapErrors(FluentValidation.Results.ValidationResult result)
        {
            return result.Errors
                .GroupBy(e => e.PropertyName).ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
        }
    }
}