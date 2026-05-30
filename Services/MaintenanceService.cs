using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Data;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger _logger;
        private readonly IBaseRepository<MaintenanceRecord> _repository;



        public MaintenanceService(ApplicationDbContext context, IAppLogger logger, IBaseRepository<MaintenanceRecord> repository)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
        }

        public async Task<PagedResult<MaintenanceListViewModel>> GetMaintenanceRecordsAsync(MaintenanceFilter filter)
        {
            IQueryable<MaintenanceRecord> query =
                _context.MaintenanceRecords
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();

                query = query.Where(m => (m.Notes != null &&
                     EF.Functions.ILike(m.Notes, $"%{search}%")) ||
                     EF.Functions.ILike(m.WorkOrder.Aircraft.TailNumber, $"%{search}%"));
            }

            int totalItems = await query.CountAsync();

            List<MaintenanceListViewModel> items = await query
                .OrderByDescending(m => m.ScheduledDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(m => new MaintenanceListViewModel
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    AircraftTailNumber = m.WorkOrder.Aircraft.TailNumber,
                    Type = m.Type,
                    Status = m.Status,
                    ScheduledDate = m.ScheduledDate,
                    CompletedDate = m.CompletedDate,
                    Notes = m.Notes
                })
                .ToListAsync();

            return new PagedResult<MaintenanceListViewModel>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalItems = totalItems
            };
        }

        public async Task<MaintenanceCreateViewModel> GetCreateViewModelAsync()
        {
            MaintenanceCreateViewModel viewModel = new()
            {
                ScheduledDate = DateTime.Now,

                WorkOrders = await _context.WorkOrders
                    .AsNoTracking()
                    .Where(w => w.Status != WorkOrderStatus.Closed) // Ensure to view to the user only the Workorder is either open or inPrgoress
                    .OrderByDescending(w => w.Id)
                    .Select(w => new SelectListItem
                    {
                        Value = w.Id.ToString(),
                        Text = $"WO-{w.Id} | {w.Aircraft.TailNumber}"
                    })
                    .ToListAsync()
            };

            return viewModel;
        }


        public async Task<MaintenanceCreateViewModel> PopulateCreateViewModelAsync(
        MaintenanceCreateViewModel viewModel)
        {

            viewModel.WorkOrders = await _context.WorkOrders
                .AsNoTracking()
                .OrderByDescending(w => w.Id)
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = $"WO-{w.Id} | {w.Aircraft.TailNumber} | {w.Description}"
                })
                .ToListAsync();

            return viewModel;
        }

        public async Task<ServiceResult<MaintenanceRecord>> CreateMaintenanceRecordAsync(MaintenanceCreateViewModel viewModel)
        {
            try
            {
                bool workOrderExists = await _context.WorkOrders.AnyAsync(w => w.Id == viewModel.WorkOrderId);

                if (!workOrderExists)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Selected work order does not exist."
                    };
                }

                DateTime scheduledDateUtc = DateTime.SpecifyKind(viewModel.ScheduledDate, DateTimeKind.Utc);

                MaintenanceRecord maintenanceRecord = new()
                {
                    WorkOrderId = viewModel.WorkOrderId,
                    Type = viewModel.Type,
                    ScheduledDate = scheduledDateUtc,
                    Status = MaintenanceStatus.Scheduled,
                    Notes = string.IsNullOrWhiteSpace(viewModel.Notes) ? null : viewModel.Notes.Trim()
                };

                await _repository.AddAsync(maintenanceRecord);

                await _repository.SaveChangesAsync();

                _logger.LogInfo("Maintenance record created successfully.",
                    new
                    {
                        maintenanceRecord.Id,
                        maintenanceRecord.WorkOrderId,
                        maintenanceRecord.Type
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = true,
                    Data = maintenanceRecord
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError("Database update failed while creating maintenance record.",
                    ex, new
                    {
                        viewModel.WorkOrderId,
                        viewModel.Type
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = false,
                    ErrorMessage = "Database update failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error while creating maintenance record.",
                    ex, new
                    {
                        viewModel.WorkOrderId,
                        viewModel.Type
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = false,
                    ErrorMessage = "Failed to create maintenance record."
                };
            }
        }


        public async Task<MaintenanceEditViewModel?> GetEditViewModelAsync(int id)
        {
            MaintenanceEditViewModel? viewModel =
                await _context.MaintenanceRecords
                    .AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MaintenanceEditViewModel
                    {
                        Id = m.Id,
                        WorkOrderId = m.WorkOrderId,
                        Type = m.Type,
                        Status = m.Status,
                        ScheduledDate = m.ScheduledDate,
                        Notes = m.Notes
                    })
                    .FirstOrDefaultAsync();

            if (viewModel is null)
            {
                return null;
            }

            return await PopulateEditViewModelAsync(viewModel);
        }

        public async Task<MaintenanceEditViewModel> PopulateEditViewModelAsync(MaintenanceEditViewModel viewModel)
        {
            viewModel.WorkOrders = await _context.WorkOrders
                .AsNoTracking()
                .OrderByDescending(w => w.Id)
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = $"WO-{w.Id} | {w.Aircraft.TailNumber} | {w.Description}"
                })
                .ToListAsync();

            return viewModel;
        }

        public async Task<ServiceResult<MaintenanceRecord>> UpdateMaintenanceRecordAsync(
            MaintenanceEditViewModel viewModel)
        {
            try
            {
                if (viewModel.Status == MaintenanceStatus.Completed)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Completed status can only be set through the Complete action."
                    };
                }

                MaintenanceRecord? record =
                    await _repository.GetByIdAsync(viewModel.Id);

                if (record is null)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Maintenance record not found."
                    };
                }

                if (record.Status == MaintenanceStatus.Completed)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Completed maintenance records cannot be edited."
                    };
                }

                record.WorkOrderId = viewModel.WorkOrderId;
                record.Type = viewModel.Type;
                record.Status = viewModel.Status;

                record.ScheduledDate =
                    DateTime.SpecifyKind(
                        viewModel.ScheduledDate,
                        DateTimeKind.Utc);

                record.Notes = string.IsNullOrWhiteSpace(viewModel.Notes)
                    ? null
                    : viewModel.Notes.Trim();


                await RecalculateWorkOrderStatusAsync(record.WorkOrderId);


                await _repository.SaveChangesAsync();

                _logger.LogInfo(
                    "Maintenance record updated successfully.",
                    new
                    {
                        record.Id,
                        record.WorkOrderId,
                        record.Status
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = true,
                    Data = record
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to update maintenance record.",
                    ex,
                    new
                    {
                        viewModel.Id
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = false,
                    ErrorMessage = "Failed to update maintenance record."
                };
            }
        }




        public async Task<MaintenanceDeleteViewModel?> GetDeleteViewModelAsync(int id)
        {
            return await _context.MaintenanceRecords.AsNoTracking().Where(m => m.Id == id)
                .Select(m => new MaintenanceDeleteViewModel
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    Type = m.Type,
                    Status = m.Status
                })
                .FirstOrDefaultAsync();
        }


        public async Task<ServiceResult<MaintenanceRecord>> DeleteMaintenanceRecordAsync(int id)
        {
            try
            {
                MaintenanceRecord? record = await _repository.GetByIdAsync(id);

                if (record is null)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Maintenance record not found."
                    };
                }

                await _repository.DeleteAsync(record);

                await RecalculateWorkOrderStatusAsync(record.WorkOrderId);
                await _repository.SaveChangesAsync();

                _logger.LogInfo(
                    "Maintenance record deleted successfully.",
                    new
                    {
                        record.Id,
                        record.WorkOrderId
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = true,
                    Data = record
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to delete maintenance record.",
                    ex,
                    new
                    {
                        Id = id
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = false,
                    ErrorMessage = "Failed to delete maintenance record."
                };
            }
        }





        public async Task<ServiceResult<MaintenanceRecord>> CompleteMaintenanceRecordAsync(int id)
        {
            try
            {
                MaintenanceRecord? record =
                    await _context.MaintenanceRecords
                        .Include(m => m.WorkOrder)
                        .FirstOrDefaultAsync(m => m.Id == id);

                if (record is null)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Maintenance record not found."
                    };
                }

                if (record.Status == MaintenanceStatus.Completed)
                {
                    return new ServiceResult<MaintenanceRecord>
                    {
                        Success = false,
                        ErrorMessage = "Maintenance record is already completed."
                    };
                }

                record.Status = MaintenanceStatus.Completed;

                record.CompletedDate = DateTime.UtcNow;

                await RecalculateWorkOrderStatusAsync(record.WorkOrderId);

                await _repository.SaveChangesAsync();

                _logger.LogInfo(
                    "Maintenance record completed.",
                    new
                    {
                        record.Id,
                        record.WorkOrderId
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = true,
                    Data = record
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to complete maintenance record.",
                    ex,
                    new
                    {
                        MaintenanceRecordId = id
                    });

                return new ServiceResult<MaintenanceRecord>
                {
                    Success = false,
                    ErrorMessage = "Failed to complete maintenance record."
                };
            }
        }



        private async Task RecalculateWorkOrderStatusAsync(int workOrderId)
        {
            WorkOrder? workOrder = await _context.WorkOrders
                .Include(w => w.MaintenanceRecords)
                .FirstOrDefaultAsync(w => w.Id == workOrderId);

            if (workOrder is null)
            {
                return;
            }

            bool allCompleted = workOrder.MaintenanceRecords.Any() && workOrder.MaintenanceRecords.All(m =>
                    m.Status == MaintenanceStatus.Completed);

            if (allCompleted)
            {
                workOrder.Status = WorkOrderStatus.Closed;
                workOrder.CompletedAt = DateTime.UtcNow;
            }
            else if (workOrder.MaintenanceRecords.Any())
            {
                workOrder.Status = WorkOrderStatus.InProgress;
                workOrder.CompletedAt = null;
            }
            else
            {
                workOrder.Status = WorkOrderStatus.Open;
                workOrder.CompletedAt = null;
            }
        }
    }
}