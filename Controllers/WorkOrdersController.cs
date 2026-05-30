using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Models;
using AircraftMRO.Models.Enums;
using AircraftMRO.Models.ViewModels.WorkOrder;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
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

        public async Task<IActionResult> Details(int id)
        {
            var result = await _workOrderService.GetDetailsAsync(id);

            if (!result.Success)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _workOrderService.GetCreateViewAsync();

            return PartialView("_Create", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkOrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Aircrafts = (await _workOrderService.GetCreateViewAsync()).Aircrafts;

                return PartialView("_Create", model);
            }

            WorkOrder workOrder = new()
            {
                AircraftId = model.AircraftId,
                Description = model.Description,
                Priority = model.Priority,
                Status = WorkOrderStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(workOrder);
            await _repository.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }



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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WorkOrderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", model);
            }
            WorkOrder? workOrder = await _repository.GetByIdAsync(model.Id);

            if (workOrder == null)
            {
                return NotFound();
            }

            workOrder.Description = model.Description;
            workOrder.Priority = model.Priority;

            await _repository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            WorkOrder? workOrder = await _repository.GetByIdAsync(id);

            if (workOrder == null)
                return NotFound();

            await _repository.DeleteAsync(workOrder);

            await _repository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}