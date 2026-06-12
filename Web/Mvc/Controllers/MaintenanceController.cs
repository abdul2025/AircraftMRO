using AircraftMRO.Common.Filters;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Application.DTOs.MaintenanceRecord;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        // =====================================================
        // LIST
        // =====================================================
        public async Task<IActionResult> Index(MaintenanceFilter filter)
        {
            var result = await _maintenanceService.GetMaintenanceRecordsAsync(filter);
            return View(result);
        }

        // =====================================================
        // DETAILS
        // =====================================================
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _maintenanceService.GetMaintenanceRecordDetailsAsync(id);

            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // =====================================================
        // CREATE
        // =====================================================
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Create()
        {
            var result = await _maintenanceService.GetCreateAsync();

            if (result == null)
                return BadRequest();

            return PartialView("_Create", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Create(MaintenanceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var repopulated = await _maintenanceService.PopulateCreateAsync(dto);
                return PartialView("_Create", repopulated);
            }

            var result = await _maintenanceService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);

                var repopulated = await _maintenanceService.PopulateCreateAsync(dto);
                return PartialView("_Create", repopulated);
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EDIT
        // =====================================================
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _maintenanceService.GetEditAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return PartialView("_Edit", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Edit(MaintenanceEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                var repopulated = await _maintenanceService.PopulateEditAsync(dto);
                return PartialView("_Edit", repopulated);
            }

            var result = await _maintenanceService.UpdateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);

                var repopulated = await _maintenanceService.PopulateEditAsync(dto);
                return PartialView("_Edit", repopulated);
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DELETE
        // =====================================================
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _maintenanceService.GetDeleteAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return PartialView("_Delete", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _maintenanceService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMPLETE
        // =====================================================
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> Complete(int id)
        {
            var result = await _maintenanceService.GetDeleteAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return PartialView("_Complete", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager},{Roles.Engineer}")]
        public async Task<IActionResult> CompleteConfirmed(int id)
        {
            var result = await _maintenanceService.CompleteAsync(id);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return RedirectToAction(nameof(Index));
        }
    }
}