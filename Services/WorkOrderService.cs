using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Common.Results;
using AircraftMRO.Data;
using AircraftMRO.Models;
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
        private readonly IAppLogger _logger;


        public WorkOrderService(ApplicationDbContext context, IAppLogger logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<PagedResult<WorkOrderListViewModel>> GetWorkOrdersAsync(WorkOrderFilter filter)
        {
            IQueryable<WorkOrder> query = _context.WorkOrders
                .AsNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string search = filter.Search.Trim();

                query = query.Where(w => // EF.Functions.ILike PostgreSQL's and Case-insensitive
                    EF.Functions.ILike(w.Description, $"%{search}%") ||
                    EF.Functions.ILike(w.Aircraft.TailNumber, $"%{search}%"));
            }

            int totalItems = await query.CountAsync();

            List<WorkOrderListViewModel> items = await query
                .OrderByDescending(w => w.CreatedAt)
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
                    CreatedAt = w.CreatedAt,
                    CompletedAt = w.CompletedAt,
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
                        CreatedAt = w.CreatedAt,
                        CompletedAt = w.CompletedAt,

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
                                Notes = m.Notes
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
    }
}