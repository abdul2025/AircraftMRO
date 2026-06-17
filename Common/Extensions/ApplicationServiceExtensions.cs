using AircraftMRO.Application.DTOs.Emails.Settings;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Application.Services;
using AircraftMRO.Repositories;
using AircraftMRO.Services;
using AircraftMRO.Services.Interfaces;
using SharedKernel.Logging.Infrastructure;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Common.Extensions
{
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        ///     Registers all application-layer services, including repositories, business logic services, 
        ///     email configuration, and logging dependencies.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="config">The <see cref="IConfiguration"/> instance to access app settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> for fluent configuration.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // This to handle the generic Log instance, controller , services ...etc.
            services.AddScoped(typeof(IAppLogger<>), typeof(CustomAppLogger<>));
            // To be used for regular stander ORM Actions 
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Domain and Business 
            services.AddScoped<IAircraftService, AircraftService>();
            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IAircraftStatusService, AircraftStatusService>();


            // Notification
            services.AddScoped<INotificationService, NotificationService>();


            // Email Service
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<EmailTemplateService>();
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

            // ... add the rest of your services here
            return services;
        }
    }
}