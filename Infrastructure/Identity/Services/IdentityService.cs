using AircraftMRO.Common.Results;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Infrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAppLogger<IdentityService> _logger;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAppLogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Authentication failed", new { Email = email });

            return new AuthResult
            {
                Succeeded = false,
                ErrorMessage = "Invalid email or password."
            };
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null)
        {
            user.LastLoginAtUtc = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);
        }

        _logger.LogInfo("User authenticated successfully",
            new
            {
                UserId = user?.Id,
                Email = email
            });

        return new AuthResult
        {
            Succeeded = true
        };
    }


    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();

        _logger.LogInfo("User logged out");
    }


    public async Task<ServiceResult<CreateUserResult>> CreateUserAsync(CreateUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmployeeNumber = request.EmployeeNumber,
            EmailConfirmed = true,
            IsActive = true
        };

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return new ServiceResult<CreateUserResult>
            {
                Success = false,
                ErrorMessage = "Email already exists."
            };
        }

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            return new ServiceResult<CreateUserResult>
            {
                Success = false,
                ErrorMessage = string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description))
            };
        }

        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return new ServiceResult<CreateUserResult>
            {
                Success = false,
                ErrorMessage = string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description))
            };
        }

        _logger.LogInfo("User created successfully",
            new
            {
                user.Id,
                user.Email,
                request.Role
            });

        return new ServiceResult<CreateUserResult>
        {
            Success = true,
            Data = new CreateUserResult
            {
                UserId = user.Id,
                FullName = user.FullName
            }
        };
    }



}