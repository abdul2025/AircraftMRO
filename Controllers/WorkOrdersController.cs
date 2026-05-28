using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models;
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

        public async Task<IActionResult> Index()
        {
            var workOrders = await _workOrderService.GetWorkOrdersAsync();

            return View(workOrders);
        }
    }
}