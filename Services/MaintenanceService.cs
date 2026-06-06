using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Data;
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
        private readonly IAppLogger<MaintenanceService> _logger;
        private readonly IBaseRepository<MaintenanceRecord> _repository;
        private readonly IAircraftStatusService _aircraftStatusService;



        public MaintenanceService(ApplicationDbContext context, IAppLogger<MaintenanceService> logger, IBaseRepository<MaintenanceRecord> repository, IAircraftStatusService aircraftStatusService)
        {
            _context = context;
            _logger = logger;
            _repository = repository;
            _aircraftStatusService = aircraftStatusService;
        }

        public async Task<PagedResult<MaintenanceListViewModel>> GetMaintenanceRecordsAsync(MaintenanceFilter filter)
        {
            IQueryable<MaintenanceRecord> query =
                _context.MaintenanceRecords
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();

                bool isIdSearch = int.TryParse(search, out int maintenanceId);

                query = query.Where(m =>
                    (isIdSearch && m.Id == maintenanceId) ||
                    (m.Notes != null &&
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


        public async Task<MaintenanceCreateViewModel> PopulateCreateViewModelAsync(MaintenanceCreateViewModel viewModel)
        {

            viewModel.WorkOrders = await _context.WorkOrders
                .AsNoTracking() // No need to keep it in memory for any Db Action
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

                DateTime scheduledDateUtc = DateTime.SpecifyKind(viewModel.ScheduledDate, DateTimeKind.Utc); // PostgresSQL expecting timestamp with time zone, Specifying the kind of the date as UTC

                MaintenanceRecord maintenanceRecord = new()
                {
                    WorkOrderId = viewModel.WorkOrderId,
                    Type = viewModel.Type,
                    ScheduledDate = scheduledDateUtc,
                    Status = MaintenanceStatus.Scheduled,
                    Notes = string.IsNullOrWhiteSpace(viewModel.Notes) ? null : viewModel.Notes.Trim()
                };

                await _repository.AddAsync(maintenanceRecord);
                await RecalculateWorkOrderStatusAsync(maintenanceRecord.WorkOrderId);

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
                    .AsNoTracking() // Just reading no tracking is required in memory 
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

        public async Task<ServiceResult<MaintenanceRecord>> UpdateMaintenanceRecordAsync(MaintenanceEditViewModel viewModel)
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

        /*
        * Work Order Status Rules
        *
        * A Work Order status is automatically derived from its associated
        * Maintenance Records.
        *
        * Open
        *  - No Maintenance Records exist.
        *
        * InProgress
        *  - One or more Maintenance Records exist.
        *  - At least one Maintenance Record is not completed.
        *
        * Closed
        *  - One or more Maintenance Records exist.
        *  - All Maintenance Records are completed.
        *
        * Examples:
        *
        * WO-100
        * └── No Maintenance Records
        *      => Open
        *
        * WO-100
        * ├── MR-1 Completed
        * └── MR-2 Scheduled
        *      => InProgress
        *
        * WO-100
        * ├── MR-1 Completed
        * └── MR-2 Completed
        *      => Closed
        */



        private async Task RecalculateWorkOrderStatusAsync(int workOrderId)
        {
            // The WorkOrder has one Aircraft but Aircraft has Many workOrder required to use ThenInclude()
            WorkOrder? workOrder = await _context.WorkOrders
                .Include(w => w.MaintenanceRecords)
                .Include(w => w.Aircraft)
                    .ThenInclude(a => a.WorkOrders) // Aircraft.WorkOrders as self-join on WorkOrders to get all workorder
                .Include(w => w.Aircraft)
                    .ThenInclude(a => a.Alerts) // Aircraft.Alerts as self-join on Alerts to get all Alerts
                .FirstOrDefaultAsync(w => w.Id == workOrderId);

            if (workOrder is null)
            {
                return;
            }

            bool allCompleted =
                workOrder.MaintenanceRecords.Any() &&
                workOrder.MaintenanceRecords.All(m =>
                    m.Status == MaintenanceStatus.Completed);

            if (allCompleted)
            {
                _logger.LogInfo(
                    "Work order automatically closed because all maintenance records are completed.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        NewStatus = WorkOrderStatus.Closed,
                        MaintenanceRecordCount = workOrder.MaintenanceRecords.Count
                    });

                workOrder.Status = WorkOrderStatus.Closed;
                workOrder.CompletedAt = DateTime.UtcNow;
            }
            else if (workOrder.MaintenanceRecords.Any())
            {
                _logger.LogInfo(
                    "Work order set to InProgress because maintenance records exist and not all are completed.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        NewStatus = WorkOrderStatus.InProgress,
                        MaintenanceRecordCount = workOrder.MaintenanceRecords.Count
                    });

                workOrder.Status = WorkOrderStatus.InProgress;
                workOrder.CompletedAt = null;
            }
            else
            {
                _logger.LogInfo(
                    "Work order set to Open because no maintenance records are associated with it.",
                    new
                    {
                        WorkOrderId = workOrder.Id,
                        NewStatus = WorkOrderStatus.Open
                    });

                workOrder.Status = WorkOrderStatus.Open;

                workOrder.CompletedAt = null;
            }



            //  Aircraft
            //  ├── WO-1 Closed
            //  ├── WO-2 Closed
            //  └── WO-3 Closed
            //       => Aircraft Active

            // Aircraft
            //  ├── WO-1 Closed
            //  ├── WO-2 Open
            //  └── WO-3 Closed
            //       => Aircraft Maintenance

            // Aircraft
            //  ├── WO-1 Closed
            //  ├── WO-2 Open and Priority as Critical 
            //  └── WO-3 Closed
            //       => Aircraft Grounded
            _aircraftStatusService.UpdateAircraftStatus(workOrder.Aircraft, workOrder.Aircraft.WorkOrders);




        }




        public async Task<MaintenanceRecordDetailsViewModel?> GetMaintenanceRecordDetailsAsync(int id)
        {
            return await _context.MaintenanceRecords
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MaintenanceRecordDetailsViewModel
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    WorkOrderDescription = m.WorkOrder.Description,
                    Type = m.Type,
                    Status = m.Status,
                    ScheduledDate = m.ScheduledDate,
                    CompletedDate = m.CompletedDate,
                    Notes = m.Notes,
                    CreatedBy = m.CreatedByUser != null ? m.CreatedByUser.FullName : null,
                    CreatedAtUtc = m.CreatedAtUtc,
                    UpdatedBy = m.UpdatedByUser != null ? m.UpdatedByUser.FullName : null,
                    UpdatedAtUtc = m.UpdatedAtUtc,
                    DeletedBy = m.DeletedByUser != null ? m.DeletedByUser.FullName : null,
                    DeletedAtUtc = m.DeletedAtUtc
                })
                .FirstOrDefaultAsync();
        }


    }
}