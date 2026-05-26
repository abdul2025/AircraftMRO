using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AircraftMRO.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(ILogger<MaintenanceController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

       
    }
}