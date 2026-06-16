using AircraftMRO.Infrastructure.Identity.DTOs;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AircraftMRO.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // token-based validation service
            var result = await _identityService.ValidateCredentialsAsync(dto.Email, dto.Password);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            return Ok(new { token = result.AccessToken });
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            // Map DTO to the Domain Request
            var request = new CreateUserRequest
            {
                FullName = dto.FullName,
                EmployeeNumber = dto.EmployeeNumber,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

            var result = await _identityService.CreateUserAsync(request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { userId = result.Data?.UserId, message = "User created successfully." });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            return Ok(new { message = "Logged out successfully." });
        }
    }
}