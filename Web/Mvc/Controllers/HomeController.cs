using Microsoft.AspNetCore.Mvc;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Controllers;

public class HomeController : Controller
{

    private readonly IAppLogger<HomeController> _logger;

    public HomeController(IAppLogger<HomeController> logger)
    {
        _logger  = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInfo("Home Page Loaded");
        return View();
    }


    
}
