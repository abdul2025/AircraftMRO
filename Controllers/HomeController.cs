using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AircraftMRO.Models;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Controllers;

public class HomeController : Controller
{

    private readonly IAppLogger _logger;

    public HomeController(IAppLogger logger)
    {
        _logger  = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInfo("Home Page Loaded");
        return View();
    }


    
}
