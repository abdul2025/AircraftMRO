using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
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
            WorkOrderEditViewModel model = await _workOrderService.GetEdutViewAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_Edit", model);
        }
    }
}