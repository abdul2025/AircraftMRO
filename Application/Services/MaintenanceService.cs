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
using AircraftMRO.Application.DTOs.MaintenanceRecord;

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

        // =========================================================
        // LIST
        // =========================================================
        public async Task<PagedResult<MaintenanceListDto>> GetMaintenanceRecordsAsync(MaintenanceFilter filter)
        {
            IQueryable<MaintenanceRecord> query = _context.MaintenanceRecords.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();
                bool isId = int.TryParse(search, out int id);

                query = query.Where(m =>
                    (isId && m.Id == id) ||
                    (m.Notes != null && EF.Functions.ILike(m.Notes, $"%{search}%")) ||
                    EF.Functions.ILike(m.WorkOrder.Aircraft.TailNumber, $"%{search}%"));
            }

            int total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.ScheduledDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(m => new MaintenanceListDto
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    AircraftTailNumber = m.WorkOrder.Aircraft.TailNumber,
                    Type = m.Type,
                    Status = m.Status,
                    ScheduledDate = m.ScheduledDate,
                    CompletedDate = m.CompletedDate,
                    Notes = m.Notes,
                    IsDeleted = m.IsDeleted 
                })
                .ToListAsync();

            return new PagedResult<MaintenanceListDto>
            {
                Items = items,
                TotalItems = total,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }


        // =========================================================
        // CREATE (GET)
        // =========================================================
        public async Task<MaintenanceCreateDto> GetCreateAsync()
        {
            return new MaintenanceCreateDto
            {
                ScheduledDate = DateTime.UtcNow,
                WorkOrders = await _context.WorkOrders
                    .AsNoTracking()
                    .Where(w => w.Status != WorkOrderStatus.Closed)
                    .Select(w => new WorkOrderLookupDto
                    {
                        Id = w.Id,
                        DisplayText = $"WO-{w.Id} | {w.Aircraft.TailNumber}"
                    })
                    .ToListAsync()
            };
        }

        public async Task<MaintenanceCreateDto> PopulateCreateAsync(MaintenanceCreateDto dto)
        {
            dto.WorkOrders = await _context.WorkOrders
                .AsNoTracking()
                .OrderByDescending(w => w.Id)
                .Select(w => new WorkOrderLookupDto
                {
                    Id = w.Id,
                    DisplayText = $"WO-{w.Id} | {w.Aircraft.TailNumber} | {w.Description}"
                })
                .ToListAsync();

            return dto;
        }




        // =========================================================
        // CREATE (POST)
        // =========================================================
        public async Task<ServiceResult<MaintenanceRecord>> CreateAsync(MaintenanceCreateDto dto)
        {
            try
            {
                bool exists = await _context.WorkOrders.AnyAsync(w => w.Id == dto.WorkOrderId);
                if (!exists)
                    return ServiceResult<MaintenanceRecord>.Failure("Work order not found.");

                var entity = new MaintenanceRecord
                {
                    WorkOrderId = dto.WorkOrderId,
                    Type = dto.Type,
                    ScheduledDate = DateTime.SpecifyKind(dto.ScheduledDate, DateTimeKind.Utc),
                    Status = MaintenanceStatus.Scheduled,
                    Notes = dto.Notes?.Trim()
                };

                await _repository.AddAsync(entity);
                await RecalculateWorkOrderStatusAsync(entity.WorkOrderId);
                await _repository.SaveChangesAsync();

                return ServiceResult<MaintenanceRecord>.SuccessResult(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Create failed", ex, dto);
                return ServiceResult<MaintenanceRecord>.Failure("Failed to create record.");
            }
        }


        // =========================================================
        // EDIT (GET)
        // =========================================================
        public async Task<ServiceResult<MaintenanceEditDto?>> GetEditAsync(int id)
        {
            var dto = await _context.MaintenanceRecords
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MaintenanceEditDto
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    Type = m.Type,
                    Status = m.Status,
                    ScheduledDate = m.ScheduledDate,
                    Notes = m.Notes
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return ServiceResult<MaintenanceEditDto?>.Failure("Not found");

            dto.WorkOrders = await _context.WorkOrders
                .AsNoTracking()
                .Select(w => new WorkOrderLookupDto
                {
                    Id = w.Id,
                    DisplayText = $"WO-{w.Id} | {w.Aircraft.TailNumber} | {w.Description}"
                })
                .ToListAsync();

            return ServiceResult<MaintenanceEditDto?>.SuccessResult(dto);
        }

        public async Task<MaintenanceEditDto> PopulateEditAsync(MaintenanceEditDto dto)
        {
            dto.WorkOrders = await _context.WorkOrders
                .AsNoTracking()
                .Select(w => new WorkOrderLookupDto
                {
                    Id = w.Id,
                    DisplayText = $"WO-{w.Id} | {w.Aircraft.TailNumber} | {w.Description}"
                })
                .ToListAsync();

            return dto;
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<ServiceResult<MaintenanceRecord>> UpdateAsync(MaintenanceEditDto dto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(dto.Id);

                if (entity == null)
                    return ServiceResult<MaintenanceRecord>.Failure("Not found");

                if (entity.Status == MaintenanceStatus.Completed)
                    return ServiceResult<MaintenanceRecord>.Failure("Already completed");

                entity.WorkOrderId = dto.WorkOrderId;
                entity.Type = dto.Type;
                entity.Status = dto.Status;
                entity.ScheduledDate = DateTime.SpecifyKind(dto.ScheduledDate, DateTimeKind.Utc);
                entity.Notes = dto.Notes?.Trim();

                await RecalculateWorkOrderStatusAsync(entity.WorkOrderId);
                await _repository.SaveChangesAsync();

                return ServiceResult<MaintenanceRecord>.SuccessResult(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError("Update failed", ex, dto);
                return ServiceResult<MaintenanceRecord>.Failure("Update failed");
            }
        }



        // =========================================================
        // DELETE (GET DTO)
        // =========================================================
        public async Task<ServiceResult<MaintenanceDeleteDto>> GetDeleteAsync(int id)
        {
            var dto = await _context.MaintenanceRecords
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MaintenanceDeleteDto
                {
                    Id = m.Id,
                    WorkOrderId = m.WorkOrderId,
                    Type = m.Type,
                    Status = m.Status
                })
                .FirstOrDefaultAsync();

            return dto == null
                ? ServiceResult<MaintenanceDeleteDto>.Failure("Not found")
                : ServiceResult<MaintenanceDeleteDto>.SuccessResult(dto);
        }

        // =========================================================
        // DELETE
        // =========================================================
        public async Task<ServiceResult<MaintenanceRecord>> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return ServiceResult<MaintenanceRecord>.Failure("Not found");

            await _repository.DeleteAsync(entity);
            await RecalculateWorkOrderStatusAsync(entity.WorkOrderId);
            await _repository.SaveChangesAsync();

            return ServiceResult<MaintenanceRecord>.SuccessResult(entity);
        }





        // =========================================================
        // COMPLETE
        // =========================================================
        public async Task<ServiceResult<MaintenanceRecord>> CompleteAsync(int id)
        {
            var entity = await _context.MaintenanceRecords
                .Include(x => x.WorkOrder)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return ServiceResult<MaintenanceRecord>.Failure("Not found");

            if (entity.Status == MaintenanceStatus.Completed)
                return ServiceResult<MaintenanceRecord>.Failure("Already completed");

            entity.Status = MaintenanceStatus.Completed;
            entity.CompletedDate = DateTime.UtcNow;

            await RecalculateWorkOrderStatusAsync(entity.WorkOrderId);
            await _repository.SaveChangesAsync();

            return ServiceResult<MaintenanceRecord>.SuccessResult(entity);
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
            var workOrder = await _context.WorkOrders
                .Include(w => w.MaintenanceRecords)
                .Include(w => w.Aircraft)
                    .ThenInclude(a => a.WorkOrders)
                .Include(w => w.Aircraft)
                    .ThenInclude(a => a.Alerts)
                .FirstOrDefaultAsync(w => w.Id == workOrderId);

            if (workOrder == null) return;

            // All Maintenance are completed
            if (workOrder.MaintenanceRecords.Any() &&
                workOrder.MaintenanceRecords.All(x => x.Status == MaintenanceStatus.Completed))
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
            await _aircraftStatusService.UpdateAircraftStatus(workOrder.Aircraft, workOrder.Aircraft.WorkOrders);
        }




        public async Task<MaintenanceDetailsDto?> GetMaintenanceRecordDetailsAsync(int id)
        {
            return await _context.MaintenanceRecords
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MaintenanceDetailsDto
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