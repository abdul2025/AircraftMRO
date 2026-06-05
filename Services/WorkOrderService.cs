using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBaseRepository<WorkOrder> _repository;
        private readonly IAppLogger _logger;
        private readonly IAircraftStatusService _aircraftStatusService;



        public WorkOrderService(ApplicationDbContext context, IAppLogger logger, IBaseRepository<WorkOrder> repository, IAircraftStatusService aircraftStatusService)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
            _aircraftStatusService = aircraftStatusService;

        }

        public async Task<PagedResult<WorkOrderListViewModel>> GetWorkOrdersAsync(WorkOrderFilter filter)
        {
            IQueryable<WorkOrder> query = _context.WorkOrders
                .AsNoTracking();

            // Search
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

            List<WorkOrderListViewModel> items = await query
                .OrderByDescending(w => w.CreatedAtUtc)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(w => new WorkOrderListViewModel
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

            return new PagedResult<WorkOrderListViewModel>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<ServiceResult<WorkOrderDetailsViewModel>> GetDetailsAsync(int id)
        {
            try
            {
                WorkOrderDetailsViewModel? model = await _context.WorkOrders
                    .AsNoTracking()
                    .Where(w => w.Id == id)
                    .Select(w => new WorkOrderDetailsViewModel
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
                            .Select(m => new MaintenanceRecordSummaryViewModel
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

                if (model is null)
                {
                    return new ServiceResult<WorkOrderDetailsViewModel>
                    {
                        Success = false,
                        ErrorMessage = "Work Order not found."
                    };
                }

                return new ServiceResult<WorkOrderDetailsViewModel>
                {
                    Success = true,
                    Data = model
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to load work order details.",
                    ex,
                    new
                    {
                        WorkOrderId = id
                    });

                return new ServiceResult<WorkOrderDetailsViewModel>
                {
                    Success = false,
                    ErrorMessage = "Failed to load work order details."
                };
            }
        }





        public async Task<WorkOrderCreateViewModel> GetCreateViewAsync()
        {
            var model = new WorkOrderCreateViewModel
            {
                Aircrafts = await _context.Aircrafts
                    .OrderBy(a => a.TailNumber)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = $"{a.TailNumber} | {a.Model}"
                    })
                    .ToListAsync()
            };
            return model;
        }


        public async Task<ServiceResult<WorkOrder>> CreateAsync(WorkOrderCreateViewModel model)
        {
            try
            {
                Aircraft? aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstOrDefaultAsync(a => a.Id == model.AircraftId);

                if (aircraft is null)
                {
                    return new ServiceResult<WorkOrder>
                    {
                        Success = false,
                        ErrorMessage = "Aircraft not found."
                    };
                }

                WorkOrder workOrder = new()
                {
                    AircraftId = model.AircraftId,
                    Description = model.Description,
                    Priority = model.Priority,
                    Status = WorkOrderStatus.Open,
                    CreatedAtUtc = DateTime.UtcNow,

                    Aircraft = aircraft
                };

                await _repository.AddAsync(workOrder);

                // Include the new work order in the in-memory collection
                IEnumerable<WorkOrder> workOrders = aircraft.WorkOrders.Append(workOrder);

                _aircraftStatusService.UpdateAircraftStatus(aircraft, workOrders);

                await _repository.SaveChangesAsync();

                _logger.LogInfo(
                    "Work order created successfully.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        AircraftId = workOrder.AircraftId,
                        Priority = workOrder.Priority
                    });

                return new ServiceResult<WorkOrder>
                {
                    Success = true,
                    Data = workOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to create work order.",
                    ex);

                return new ServiceResult<WorkOrder>
                {
                    Success = false,
                    ErrorMessage = "Failed to create work order."
                };
            }
        }



        public async Task<ServiceResult<WorkOrderEditViewModel>> GetEditViewAsync(int id)
        {
            WorkOrderEditViewModel? model = await _context.WorkOrders
                .AsNoTracking()
                .Where(w => w.Id == id)
                .Select(w => new WorkOrderEditViewModel
                {
                    Id = w.Id,
                    AircraftId = w.AircraftId,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description,
                    Priority = w.Priority,
                })
                .FirstOrDefaultAsync();

            if (model is null)
            {
                return new ServiceResult<WorkOrderEditViewModel>
                {
                    Success = false,
                    ErrorMessage = "Work Order not found."
                };
            }

            return new ServiceResult<WorkOrderEditViewModel>
            {
                Success = true,
                Data = model
            };
        }


        public async Task<ServiceResult<WorkOrder>> EditAsync(WorkOrderEditViewModel model)
        {
            try
            {
                WorkOrder? workOrder = await _repository.GetByIdAsync(model.Id);

                if (workOrder is null)
                {
                    return new ServiceResult<WorkOrder>
                    {
                        Success = false,
                        ErrorMessage = "Work Order not found."
                    };
                }

                WorkOrderPriority previousPriority = workOrder.Priority;

                workOrder.Description = model.Description;
                workOrder.Priority = model.Priority;

                Aircraft aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstAsync(a => a.Id == workOrder.AircraftId);


                _aircraftStatusService.UpdateAircraftStatus(aircraft, aircraft.WorkOrders);

                await _repository.SaveChangesAsync();

                _logger.LogInfo(
                    "Work order updated successfully.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        PreviousPriority = previousPriority,
                        NewPriority = workOrder.Priority
                    });

                return new ServiceResult<WorkOrder>
                {
                    Success = true,
                    Data = workOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to update work order.",
                    ex,
                    new
                    {
                        WorkOrderId = model.Id
                    });

                return new ServiceResult<WorkOrder>
                {
                    Success = false,
                    ErrorMessage = "Failed to update work order."
                };
            }
        }

        public async Task<ServiceResult<WorkOrderDeleteViewModel>> GetDeleteViewAsync(int id)
        {
            WorkOrderDeleteViewModel? model = await _context.WorkOrders
                .AsNoTracking()
                .Where(w => w.Id == id)
                .Select(w => new WorkOrderDeleteViewModel
                {
                    Id = w.Id,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description
                })
                .FirstOrDefaultAsync();

            if (model is null)
            {
                return new ServiceResult<WorkOrderDeleteViewModel>
                {
                    Success = false,
                    ErrorMessage = "Work Order not found."
                };
            }

            return new ServiceResult<WorkOrderDeleteViewModel>
            {
                Success = true,
                Data = model
            };
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
                    return new ServiceResult<WorkOrder>
                    {
                        Success = false,
                        ErrorMessage = "Work Order not found."
                    };
                }

                await _repository.DeleteAsync(workOrder);

                Aircraft aircraft = await _context.Aircrafts
                    .Include(a => a.WorkOrders)
                    .FirstAsync(a => a.Id == workOrder.AircraftId);


                IEnumerable<WorkOrder> CurrentWorkOrderInDB = aircraft.WorkOrders.Where(w => w.Id != workOrder.Id);
                _aircraftStatusService.UpdateAircraftStatus(aircraft, CurrentWorkOrderInDB);


                await _repository.SaveChangesAsync();

                _logger.LogInfo("Work order deleted successfully.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        AircraftId = workOrder.AircraftId
                    });

                return new ServiceResult<WorkOrder>
                {
                    Success = true,
                    Data = workOrder
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete work order.",
                    ex,
                    new
                    {
                        WorkOrderId = id
                    });

                return new ServiceResult<WorkOrder>
                {
                    Success = false,
                    ErrorMessage = "Failed to delete work order."
                };
            }
        }



    }
}