using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Domain;
using AircraftMRO.Models.ViewModels.Aircraft;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class AircraftController : Controller
    {
        private readonly IAircraftService _aircraftService;
        private readonly IBaseRepository<Aircraft> _repository;

        public AircraftController(IAircraftService aircraftService, IBaseRepository<Aircraft> repository)
        {
            _aircraftService = aircraftService;
            _repository = repository;
        }

        public async Task<IActionResult> Index(AircraftFilter filter)
        {
            var aircrafts = await _aircraftService.GetAircraftAsync(filter);

            return View(aircrafts);
        }

        
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Details(int id)
        {
            ServiceResult<AircraftDetailsViewModel> result = await _aircraftService.GetAircraftDetailsAsync(id);

            if (!result.Success)
            {
                return NotFound();
            }

            return View(result.Data);
        }


        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public IActionResult Create()
        {
            return PartialView("_Create", new AircraftCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create(AircraftCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", viewModel);
            }

            ServiceResult<Aircraft> result = await _aircraftService.CreateAircraftAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    nameof(viewModel.TailNumber),
                    result.ErrorMessage!);

                return PartialView("_Create", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Edit(int id)
        {
            Aircraft? aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return NotFound();
            }

            AircraftEditViewModel viewModel = new()
            {
                Id = aircraft.Id,
                TailNumber = aircraft.TailNumber,
                Model = aircraft.Model,
                Manufacturer = aircraft.Manufacturer
            };

            return PartialView("_Edit", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Edit(AircraftEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", viewModel);
            }

            Aircraft? aircraft = await _repository.GetByIdAsync(viewModel.Id);

            if (aircraft == null)
            {
                return NotFound();
            }

            aircraft.Model = viewModel.Model;
            aircraft.Manufacturer = viewModel.Manufacturer;

            await _repository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            Aircraft? aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return NotFound();
            }

            AircraftDeleteViewModel viewModel = new()
            {
                Id = aircraft.Id,
                TailNumber = aircraft.TailNumber,
                Model = aircraft.Model
            };

            return PartialView("_Delete", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Aircraft? aircraft = await _repository.GetByIdAsync(id);

            if (aircraft == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(aircraft);

            await _repository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}