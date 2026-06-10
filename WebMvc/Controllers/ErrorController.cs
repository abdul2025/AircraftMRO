using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // to avoid caching in browser, where every time the user visit will get a fresh response not the cached pages/Content
        public IActionResult Error()
        {
            return View("500");
        }

        [Route("Error/{statusCode}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // to avoid caching in browser, where every time the user visit will get a fresh response not the cached pages/Content
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