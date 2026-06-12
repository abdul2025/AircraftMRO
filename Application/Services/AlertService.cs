using AircraftMRO.Application.DTOs.Alert;
using AircraftMRO.Common.Pagination;
using AircraftMRO.Domain;
using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Services
{
    public class AlertService : IAlertService
    {
        private readonly ApplicationDbContext _context;

        public AlertService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<AlertListDto>> GetAlertsAsync(string? search, bool? resolved, int pageNumber, int pageSize)
        {
            IQueryable<Alert> query = _context.Alerts.Include(a => a.Aircraft).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Title.Contains(search) ||
                    a.Message.Contains(search) ||
                    a.Aircraft.TailNumber.Contains(search));
            }

            if (resolved.HasValue)
            {
                query = query.Where(a => (a.ResolvedAt != null) == resolved.Value);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlertListDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    AircraftTailNumber = a.Aircraft.TailNumber,
                    Severity = a.Severity,
                    CreatedAt = a.CreatedAtUtc,
                    ResolvedAt = a.ResolvedAt,
                    IsResolved = a.ResolvedAt != null,
                    NotificationSent = a.NotificationSent,
                    WorkOrderIds = a.WorkOrderIds
                })
                .ToListAsync();

            return new PagedResult<AlertListDto>
            {
                Items = items,
                TotalItems = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}