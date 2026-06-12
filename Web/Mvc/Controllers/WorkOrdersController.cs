using AircraftMRO.Application.DTOs.WorkOrder; // Swapped ViewModels for DTOs
using AircraftMRO.Common.Filters;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Domain;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class WorkOrdersController : Controller
    {
        private readonly IWorkOrderService _workOrderService;

        public WorkOrdersController(IWorkOrderService workOrderService)
        {
            _workOrderService = workOrderService;
        }

        public async Task<IActionResult> Index(WorkOrderFilter filter)
        {
            var workOrders = await _workOrderService.GetWorkOrdersAsync(filter);

            return View(workOrders);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _workOrderService.GetDetailsAsync(id);

            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }
        
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create()
        {
            var dto = await _workOrderService.GetCreateDtoAsync();

            return PartialView("_Create", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create(WorkOrderCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the dropdown list on validation failure
                dto.Aircrafts = (await _workOrderService.GetCreateDtoAsync()).Aircrafts;

                return PartialView("_Create", dto);
            }

            var result = await _workOrderService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty,
                    result.ErrorMessage ?? "Failed to create work order.");

                // Re-populate the dropdown list on error
                dto.Aircrafts = (await _workOrderService.GetCreateDtoAsync()).Aircrafts;

                return PartialView("_Create", dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _workOrderService.GetEditAsync(id);

            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", result.Data);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkOrderEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the dropdown list on validation failure
                dto = await _workOrderService.PopulateEditAsync(dto);
                return PartialView("_Edit", dto);
            }

            var result = await _workOrderService.EditAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to edit work order.");
                
                // Re-populate the dropdown list on error
                dto = await _workOrderService.PopulateEditAsync(dto);
                return PartialView("_Edit", dto);
            }

            TempData["SuccessMessage"] = "Work order updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _workOrderService.GetDeleteAsync(id);

            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _workOrderService.DeleteAsync(id);

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}