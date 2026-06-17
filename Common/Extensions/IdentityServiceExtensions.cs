using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Infrastructure.Identity.Services;
using AircraftMRO.Infrastructure.Identity.Services.Interfaces;
using AircraftMRO.Services;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AircraftMRO.Common.Extensions
{
    public static class IdentityServiceExtensions
    {
        /// <summary>
        /// Configures and registers core Identity services, including token management, 
        /// user identity management, and Entity Framework stores.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        /// <remarks>
        /// This extension method sets up:
        /// <list type="bullet">
        /// <item><description>Scoped <see cref="ITokenService"/> and <see cref="IIdentityService"/>.</description></item>
        /// <item><description>Default Identity password policies (min length 6, requires digits).</description></item>
        /// <item><description>Entity Framework stores and default token providers.</description></item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddHttpContextAccessor(); // allows you to access the current HTTP request from anywhere
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            // options.Password.RequiredUniqueChars = 4;
            // options.Password.RequireUppercase = true;
            // options.Password.RequireLowercase = true;
            // options.Password.RequireNonAlphanumeric = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

            return services;
        }
    }
}