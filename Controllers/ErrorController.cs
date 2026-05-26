using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("500");
        }

        [Route("Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return statusCode switch
            {
                404 => View("404"),
                403 => View("403"),
                _ => View("Error")
            };
        }
    }
}