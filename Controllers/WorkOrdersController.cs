using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AircraftMRO.Controllers
{
    public class WorkOrdersController : Controller
    {
        private readonly ILogger<WorkOrdersController> _logger;
        private readonly IBaseRepository<WorkOrder> _repository;
        private readonly IWorkOrderService _workOrderService;


        public WorkOrdersController(ILogger<WorkOrdersController> logger, IBaseRepository<WorkOrder> repository, IWorkOrderService workOrderService)
        {
            _logger = logger;
            _repository = repository;
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

            if (!result.Success)
            {
                return NotFound();
            }

            return View(result.Data);
        }
        
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create()
        {
            var model = await _workOrderService.GetCreateViewAsync();

            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create(WorkOrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Aircrafts = (await _workOrderService.GetCreateViewAsync()).Aircrafts;

                return PartialView("_Create", model);
            }

            var result = await _workOrderService.CreateAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty,
                    result.ErrorMessage ?? "Failed to create work order.");

                model.Aircrafts = (await _workOrderService.GetCreateViewAsync()).Aircrafts;

                return PartialView("_Create", model);
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ServiceResult<WorkOrderEditViewModel> result = await _workOrderService.GetEditViewAsync(id);

            if (!result.Success)
            {
                return NotFound();
            }

            return PartialView("_Edit", result.Data);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkOrderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }

            ServiceResult<WorkOrder> result = await _workOrderService.EditAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);

                return PartialView("_Edit", model);
            }

            TempData["SuccessMessage"] = "Work order updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _workOrderService.GetDeleteViewAsync(id);

            if (!result.Success)
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