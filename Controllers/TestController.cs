
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Test404()
        {
            return NotFound();
        }

        public IActionResult Test403()
        {
            return StatusCode(403);
        }

        public IActionResult Test500()
        {
            throw new Exception("500");
        }
    }
}