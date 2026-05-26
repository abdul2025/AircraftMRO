using Microsoft.AspNetCore.Mvc;
using SharedKernel.Logging.Interfaces;
using AircraftMRO.Services.Interfaces;
using AircraftMRO.Models.ViewModels.Aircraft;
using AircraftMRO.Models;

namespace AircraftMRO.Controllers
{
    public class AircraftController : Controller
    {
        private readonly IAppLogger _logger;
        private readonly IAircraftService _aircraftService;




        public AircraftController(IAppLogger logger, IAircraftService aircraftService)
        {
            _logger = logger;
            _aircraftService = aircraftService;
        }

        public async Task<IActionResult> Index()
        {
            var aircrafts = await _aircraftService.AsyncGetAircraft();
            return View(aircrafts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var aircrafts = await _aircraftService.AsyncGetAircraftDetails(id);
            return View(aircrafts);
        }


        public IActionResult Create()
        {
            return PartialView("_Create", new AircraftCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AircraftCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Create", model);

            // await _aircraftService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }






        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Aircraft? aircraft = await _context.Aircraft.FindAsync(id);

            if (aircraft == null)
                return NotFound();

            AircraftEditViewModel model = new()
            {
                Id = aircraft.Id,
                TailNumber = aircraft.TailNumber,
                Model = aircraft.Model,
                Manufacturer = aircraft.Manufacturer
            };

            return PartialView("_Edit", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AircraftEditViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Edit", model);

            Aircraft? aircraft = await _context.Aircraft.FindAsync(model.Id);

            if (aircraft == null)
                return NotFound();

            aircraft.TailNumber = model.TailNumber;
            aircraft.Model = model.Model;
            aircraft.Manufacturer = model.Manufacturer;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Aircraft? aircraft = await _context.Aircraft.FindAsync(id);

            if (aircraft == null)
                return NotFound();

            AircraftDeleteViewModel model = new()
            {
                Id = aircraft.Id,
                TailNumber = aircraft.TailNumber,
                Model = aircraft.Model
            };

            return PartialView("_Delete", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(AircraftDeleteViewModel model)
        {
            Aircraft? aircraft = await _context.Aircraft.FindAsync(model.Id);

            if (aircraft == null)
                return NotFound();

            _context.Aircraft.Remove(aircraft);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}