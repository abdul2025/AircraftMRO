using AircraftMRO.Application.DTOs.WorkOrder; // Added
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Domain;
using AircraftMRO.Domain.Enums;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBaseRepository<WorkOrder> _repository;
        private readonly IAppLogger<WorkOrderService> _logger;
        private readonly IAircraftStatusService _aircraftStatusService;

        public WorkOrderService(ApplicationDbContext context, IAppLogger<WorkOrderService> logger, IBaseRepository<WorkOrder> repository, IAircraftStatusService aircraftStatusService)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
            _aircraftStatusService = aircraftStatusService;
        }

        public async Task<PagedResult<WorkOrderListDto>> GetWorkOrdersAsync(WorkOrderFilter filter)
        {
            IQueryable<WorkOrder> query = _context.WorkOrders.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();
                bool isIdSearch = int.TryParse(search, out int id);

                query = query.Where(w =>
                    (isIdSearch && w.Id == id) ||
                    EF.Functions.ILike(w.Description, $"%{search}%") ||
                    EF.Functions.ILike(w.Aircraft.TailNumber, $"%{search}%"));
            }

            int totalItems = await query.CountAsync();

            List<WorkOrderListDto> items = await query
                .OrderByDescending(w => w.CreatedAtUtc)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(w => new WorkOrderListDto
                {
                    Id = w.Id,
                    AircraftId = w.Aircraft.Id,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    CreatedAtUtc = w.CreatedAtUtc,
                    CompletedAt = w.CompletedAt,
                    IsDeleted = w.IsDeleted,
                    MaintenanceRecordCount = w.MaintenanceRecords.Count()
                })
                .ToListAsync();

            return new PagedResult<WorkOrderListDto>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<ServiceResult<WorkOrderDetailsDto>> GetDetailsAsync(int id)
        {
            try
            {
                WorkOrderDetailsDto? dto = await _context.WorkOrders
                    .AsNoTracking()
                    .Where(w => w.Id == id)
                    .Select(w => new WorkOrderDetailsDto
                    {
                        Id = w.Id,
                        AircraftId = w.AircraftId,
                        AircraftTailNumber = w.Aircraft.TailNumber,
                        Description = w.Description,
                        Priority = w.Priority,
                        Status = w.Status,
                        CreatedAtUtc = w.CreatedAtUtc,
                        CompletedAt = w.CompletedAt,
                        IsDeleted = w.IsDeleted,
                        CreatedBy = w.CreatedByUser != null ? w.CreatedByUser.FullName : null,
                        UpdatedBy = w.UpdatedByUser != null ? w.UpdatedByUser.FullName : null,
                        UpdatedAtUtc = w.UpdatedAtUtc,
                        DeletedBy = w.DeletedByUser != null ? w.DeletedByUser.FullName : null,
                        DeletedAtUtc = w.DeletedAtUtc,
                        MaintenanceRecordCount = w.MaintenanceRecords.Count(),
                        MaintenanceRecords = w.MaintenanceRecords
                            .OrderByDescending(m => m.ScheduledDate)
                            .Select(m => new MaintenanceRecordSummaryDto
                            {
                                Id = m.Id,
                                Type = m.Type,
                                Status = m.Status,
                                ScheduledDate = m.ScheduledDate,
                                CompletedDate = m.CompletedDate,
                                Notes = m.Notes,
                                IsDeleted = m.IsDeleted
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (dto is null)
                {
                    return new ServiceResult<WorkOrderDetailsDto> { Success = false, ErrorMessage = "Work Order not found." };
                }

                return new ServiceResult<WorkOrderDetailsDto> { Success = true, Data = dto };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load work order details.", ex, new { WorkOrderId = id });
                return new ServiceResult<WorkOrderDetailsDto> { Success = false, ErrorMessage = "Failed to load work order details." };
            }
        }

        public async Task<WorkOrderCreateDto> GetCreateDtoAsync()
        {
            return new WorkOrderCreateDto
            {
                Aircrafts = await _context.Aircrafts
                    .AsNoTracking()
                    .OrderBy(a => a.TailNumber)
                    .Select(a => new AircraftLookupDto
                    {
                        Id = a.Id,
                        DisplayText = $"{a.TailNumber} | {a.Model}"
                    })
                    .ToListAsync()
            };
        }

        public async Task<ServiceResult<WorkOrder>> CreateAsync(WorkOrderCreateDto dto)
        {
            try
            {
                Aircraft? aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstOrDefaultAsync(a => a.Id == dto.AircraftId);

                if (aircraft is null)
                {
                    return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Aircraft not found." };
                }

                WorkOrder workOrder = new()
                {
                    AircraftId = dto.AircraftId,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    Status = WorkOrderStatus.Open,
                    CreatedAtUtc = DateTime.UtcNow,
                    Aircraft = aircraft
                };

                await _repository.AddAsync(workOrder);

                IEnumerable<WorkOrder> workOrders = aircraft.WorkOrders.Append(workOrder);
                _aircraftStatusService.UpdateAircraftStatus(aircraft, workOrders);

                await _repository.SaveChangesAsync();

                _logger.LogInfo("Work order created successfully.", new { WorkOrderId = workOrder.Id, AircraftId = workOrder.AircraftId, Priority = workOrder.Priority });

                return new ServiceResult<WorkOrder> { Success = true, Data = workOrder };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create work order.", ex);
                return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Failed to create work order." };
            }
        }

        public async Task<ServiceResult<WorkOrderEditDto>> GetEditAsync(int id)
        {
            WorkOrderEditDto? dto = await _context.WorkOrders
                .AsNoTracking()
                .Where(w => w.Id == id)
                .Select(w => new WorkOrderEditDto
                {
                    Id = w.Id,
                    AircraftId = w.AircraftId,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description,
                    Priority = w.Priority
                })
                .FirstOrDefaultAsync();

            if (dto is null)
            {
                return new ServiceResult<WorkOrderEditDto> { Success = false, ErrorMessage = "Work Order not found." };
            }

            // Populate the dropdown list before sending it to the UI
            dto = await PopulateEditAsync(dto);

            return new ServiceResult<WorkOrderEditDto> { Success = true, Data = dto };
        }

        public async Task<WorkOrderEditDto> PopulateEditAsync(WorkOrderEditDto dto)
        {
            dto.Aircrafts = await _context.Aircrafts
                .AsNoTracking()
                .OrderBy(a => a.TailNumber)
                .Select(a => new AircraftLookupDto
                {
                    Id = a.Id,
                    DisplayText = $"{a.TailNumber} | {a.Model}"
                })
                .ToListAsync();

            return dto;
        }

        public async Task<ServiceResult<WorkOrder>> EditAsync(WorkOrderEditDto dto)
        {
            try
            {
                WorkOrder? workOrder = await _repository.GetByIdAsync(dto.Id);

                if (workOrder is null)
                {
                    return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Work Order not found." };
                }

                WorkOrderPriority previousPriority = workOrder.Priority;

                // Update the entity properties
                workOrder.Description = dto.Description;
                workOrder.Priority = dto.Priority;

                // If your UI allows the user to change the Aircraft during an edit
                if (workOrder.AircraftId != dto.AircraftId)
                {
                    workOrder.AircraftId = dto.AircraftId;
                }

                Aircraft aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstAsync(a => a.Id == workOrder.AircraftId);

                _aircraftStatusService.UpdateAircraftStatus(aircraft, aircraft.WorkOrders);

                await _repository.SaveChangesAsync();

                _logger.LogInfo("Work order updated successfully.", new { WorkOrderId = workOrder.Id, PreviousPriority = previousPriority, NewPriority = workOrder.Priority });

                return new ServiceResult<WorkOrder> { Success = true, Data = workOrder };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update work order.", ex, new { WorkOrderId = dto.Id });
                return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Failed to update work order." };
            }
        }

        public async Task<ServiceResult<WorkOrderDeleteDto>> GetDeleteAsync(int id)
        {
            WorkOrderDeleteDto? dto = await _context.WorkOrders
                .AsNoTracking()
                .Where(w => w.Id == id)
                .Select(w => new WorkOrderDeleteDto
                {
                    Id = w.Id,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description
                })
                .FirstOrDefaultAsync();

            if (dto is null)
            {
                return new ServiceResult<WorkOrderDeleteDto> { Success = false, ErrorMessage = "Work Order not found." };
            }

            return new ServiceResult<WorkOrderDeleteDto> { Success = true, Data = dto };
        }

        public async Task<ServiceResult<WorkOrder>> DeleteAsync(int id)
        {
            try
            {
                WorkOrder? workOrder = await _context.WorkOrders
                    .Include(w => w.Aircraft)
                        .ThenInclude(a => a.WorkOrders)
                    .FirstOrDefaultAsync(w => w.Id == id);

                if (workOrder is null)
                {
                    return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Work Order not found." };
                }

                await _repository.DeleteAsync(workOrder);

                Aircraft aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstAsync(a => a.Id == workOrder.AircraftId);

                IEnumerable<WorkOrder> currentWorkOrderInDb = aircraft.WorkOrders.Where(w => w.Id != workOrder.Id);
                _aircraftStatusService.UpdateAircraftStatus(aircraft, currentWorkOrderInDb);

                await _repository.SaveChangesAsync();

                _logger.LogInfo("Work order deleted successfully.", new { WorkOrderId = workOrder.Id, AircraftId = workOrder.AircraftId });

                return new ServiceResult<WorkOrder> { Success = true, Data = workOrder };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete work order.", ex, new { WorkOrderId = id });
                return new ServiceResult<WorkOrder> { Success = false, ErrorMessage = "Failed to delete work order." };
            }
        }
    }
}