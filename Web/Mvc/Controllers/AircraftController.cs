using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Domain;
using AircraftMRO.Mvc.ViewModels.Aircraft;
using AircraftMRO.Repositories;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AircraftMRO.Application.DTOs.Aircraft;
using AircraftMRO.Web.Mvc.Mappings;

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

        //
        //   View
        //

        public async Task<IActionResult> Index(AircraftFilter filter)
        {
            var aircrafts = await _aircraftService.GetAircraftAsync(filter);

            return View(aircrafts);
        }


        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Details(int id)
        {
            ServiceResult<AircraftDetailsDto> result = await _aircraftService.GetAircraftDetailsAsync(id);

            if (!result.Success || result.Data == null)
            {
                return NotFound();
            }

            var vm = AircraftMappings.ToVm(result.Data);

            return View(vm);
        }


        //
        //   Create
        //
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public IActionResult Create()
        {
            return PartialView("_Create", new AircraftCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Create(AircraftCreateDto dto)
        {
            var result = await _aircraftService.CreateAircraftAsync(dto);

            if (!result.Success)
            {
                if (result.ValidationErrors != null)
                {
                    foreach (var error in result.ValidationErrors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage!);
                }

                return PartialView("_Create", dto);
            }

            return RedirectToAction(nameof(Index));
        }


        //
        //   EDIT
        //
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _aircraftService.GetForEditAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return PartialView("_Edit", result.Data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Edit(AircraftEditDto dto)
        {
            var result = await _aircraftService.UpdateAsync(dto);

            if (!result.Success)
            {
                if (result.ValidationErrors != null)
                {
                    foreach (var error in result.ValidationErrors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage!);
                }

                return PartialView("_Edit", dto);
            }

            return RedirectToAction(nameof(Index));
        }


        //
        //   DELETE
        //
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _aircraftService.GetForDeleteAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return PartialView("_Delete", result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _aircraftService.DeleteAsync(id);

            if (!result.Success)
            {
                // optional: you can show error view or toast later
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}