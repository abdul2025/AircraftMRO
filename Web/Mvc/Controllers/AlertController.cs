using AircraftMRO.Common.Pagination;
using AircraftMRO.Domain;
using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Mvc.ViewModels.Alert;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Controllers
{
    public class AlertController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Index(
            string? search,
            bool? resolved,
            int pageNumber = 1,
            int pageSize = 10)
        {
            IQueryable<Alert> query = _context.Alerts
                .Include(a => a.Aircraft)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Title.Contains(search) ||
                    a.Message.Contains(search) ||
                    a.Aircraft.TailNumber.Contains(search));
            }

            if (resolved.HasValue)
            {
                query = query.Where(a => a.ResolvedAt != null == resolved.Value);
            }

            int totalCount = await query.CountAsync();

            List<AlertListViewModel> items = await query
                .OrderByDescending(a => a.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlertListViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    AircraftTailNumber = a.Aircraft.TailNumber,
                    Severity = a.Severity,
                    IsResolved = a.ResolvedAt != null,
                    ResolvedAt = a.ResolvedAt,
                    NotificationSent = a.NotificationSent,
                    WorkOrderIds = a.WorkOrderIds
                })
                .ToListAsync();

            PagedResult<AlertListViewModel> result = new()
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(result);
        }
    }
}