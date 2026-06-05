using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Common.Filters;
using AircraftMRO.Common.Results;
using AircraftMRO.Models;
using AircraftMRO.Models.ViewModels.MaintenanceRecord;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> Create()
        {
            MaintenanceCreateViewModel viewModel = await _maintenanceService.GetCreateViewModelAsync();

            return PartialView("_Create", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceCreateViewModel viewModel)
        {
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
        public async Task<IActionResult> Complete(int id)
        {
            MaintenanceDeleteViewModel? viewModel = await _maintenanceService.GetDeleteViewModelAsync(id);

            if (viewModel is null)
            {
                return NotFound();
            }

            return PartialView("_Complete", viewModel);
        }


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