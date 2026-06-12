using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Domain;
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

        public AircraftController(IAircraftService aircraftService, IBaseRepository<Aircraft> repository)
        {
            _aircraftService = aircraftService;
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
            // 1. Let ASP.NET Core & FluentValidation automatically block bad data!
            if (!ModelState.IsValid)
            {
                return PartialView("_Create", dto);
            }

            // 2. Data is structurally valid, send it to the service
            var result = await _aircraftService.CreateAircraftAsync(dto);

            // 3. Handle Business Rules / Database Errors (e.g., Duplicate Tail Number)
            if (!result.Success)
            {
                // We no longer need the complex dictionary mapping loop!
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
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
            // 1. Automatic FluentValidation check
            if (!ModelState.IsValid)
            {
                return PartialView("_Edit", dto);
            }

            // 2. Send valid data to the service
            var result = await _aircraftService.UpdateAsync(dto);

            // 3. Handle 'Not Found' or DB Errors
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                return PartialView("_Edit", dto);
            }

            TempData["SuccessMessage"] = "Aircraft updated successfully.";

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
            // TODO: Requiired to validate how the delete is working along side with soft Delete because dele action keeps occurs regardless of the soft delete 
            var result = await _aircraftService.DeleteAsync(id);

            if (!result.Success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}