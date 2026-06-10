
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Domain;
using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Mvc.ViewModels.MaintenanceRecord;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;


        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }


        public async Task<IActionResult> Index(MaintenanceFilter filter)
        {
            var maintenanceRecords = await _maintenanceService.GetMaintenanceRecordsAsync(filter);

            return View(maintenanceRecords);
        }


        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Details(int id)
        {
            MaintenanceRecordDetailsViewModel? model = await _maintenanceService.GetMaintenanceRecordDetailsAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);

        }


        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Create()
        {
            MaintenanceCreateViewModel viewModel = await _maintenanceService.GetCreateViewModelAsync();

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Create(MaintenanceCreateViewModel viewModel)
        {
            Console.WriteLine(viewModel.ScheduledDate.Kind);
            if (!ModelState.IsValid)
            {
                viewModel = await _maintenanceService.PopulateCreateViewModelAsync(viewModel);

                return PartialView("_Create", viewModel);
            }

            var result = await _maintenanceService.CreateMaintenanceRecordAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage!);

                viewModel = await _maintenanceService.PopulateCreateViewModelAsync(viewModel);

                return PartialView("_Create", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]

        public async Task<IActionResult> Edit(int id)
        {
            MaintenanceEditViewModel? viewModel =
                await _maintenanceService.GetEditViewModelAsync(id);

            if (viewModel is null)
            {
                return NotFound();
            }

            return PartialView("_Edit", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Edit(MaintenanceEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel = await _maintenanceService.PopulateEditViewModelAsync(viewModel);
                return PartialView("_Edit", viewModel);
            }

            ServiceResult<MaintenanceRecord> result = await _maintenanceService.UpdateMaintenanceRecordAsync(viewModel);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                viewModel = await _maintenanceService.PopulateEditViewModelAsync(viewModel);
                return PartialView("_Edit", viewModel);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> Delete(int id)
        {
            MaintenanceDeleteViewModel? viewModel = await _maintenanceService.GetDeleteViewModelAsync(id);

            if (viewModel is null)
            {
                return NotFound();
            }

            return PartialView("_Delete", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ServiceResult<MaintenanceRecord> result = await _maintenanceService.DeleteMaintenanceRecordAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        public async Task<IActionResult> Complete(int id)
        {
            MaintenanceDeleteViewModel? viewModel = await _maintenanceService.GetDeleteViewModelAsync(id);

            if (viewModel is null)
            {
                return NotFound();
            }

            return PartialView("_Complete", viewModel);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.MaintenanceManager}, {Roles.Engineer}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteConfirmed(int id)
        {
            ServiceResult<MaintenanceRecord> result = await _maintenanceService.CompleteMaintenanceRecordAsync(id);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}