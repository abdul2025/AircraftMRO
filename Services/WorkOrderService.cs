using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Data;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ApplicationDbContext _context;

        public WorkOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkOrderListViewModel>>
            GetWorkOrdersAsync()
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .Select(w => new WorkOrderListViewModel
                {
                    Id = w.Id,
                    AircraftId = w.Aircraft.Id,
                    AircraftTailNumber = w.Aircraft.TailNumber,
                    Description = w.Description,
                    Priority = w.Priority,
                    Status = w.Status,
                    CreatedAt = w.CreatedAt,
                    CompletedAt = w.CompletedAt
                })
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}