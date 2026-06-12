using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    
    public class AlertController : Controller
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Index(string? search, bool? resolved, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _alertService.GetAlertsAsync(search, resolved, pageNumber, pageSize);

            return View(result);
        }
    }
}
