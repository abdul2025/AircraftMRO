using AircraftMRO.Infrastructure.Identity.Constants;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Identity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedKernel.Logging.Interfaces;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Infrastructure.Models;

namespace AircraftMRO.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IAppLogger<AccountController> _logger;

        public AccountController(IIdentityService identityService, IAppLogger<AccountController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        // Login GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        // Login POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _identityService.AuthenticateAsync(model.Email, model.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid email or password.");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }


        // Create GET
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateUserViewModel();

            PopulateRoles(model);

            return PartialView("_Create", model);
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateRoles(model);

                return PartialView("_Create", model);
            }

            var request = new CreateUserRequest
            {
                FullName = model.FullName,
                EmployeeNumber = model.EmployeeNumber,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            var result = await _identityService.CreateUserAsync(request);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Failed to create user.");
                PopulateRoles(model);
                return PartialView("_Create", model);
            }
            
            ModelState.Clear();
            var newModel = new CreateUserViewModel();

            PopulateRoles(newModel);

            ViewBag.SuccessMessage = $"User '{model.FullName}' created successfully.";

            return PartialView("_Create", newModel);
        }
        // Helper Function 
        private static void PopulateRoles(CreateUserViewModel model)
        {
            model.Roles = Roles.All.Select(r => new SelectListItem
            {
                Value = r,
                Text = r
            });
        }


        // LOGOUT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _identityService.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}