using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Services.Interfaces.INotification;
using FluentEmail.Core;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly IAppLogger<SmtpEmailService> _logger;

        // FluentEmail provides IFluentEmail automatically once registered in Program.cs
        public SmtpEmailService(IFluentEmail fluentEmail, IAppLogger<SmtpEmailService> logger)
        {
            _fluentEmail = fluentEmail;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Leverage FluentEmail's clean builder syntax
                var response = await _fluentEmail
                    .To(toEmail)
                    .Subject(subject)
                    .Body(body, isHtml: true) // Enforces clean HTML layouts
                    .SendAsync();

                if (!response.Successful)
                {
                    var errors = string.Join(", ", response.ErrorMessages);
                    throw new Exception($"FluentEmail gateway returned error status: {errors}");
                }

                _logger.LogInfo("Email dispatched successfully via FluentEmail gateway.", new { To = toEmail, Subject = subject });
            }
            catch (Exception ex)
            {
                _logger.LogError("Critical failure in FluentEmail transmission loop", ex, new { To = toEmail });
                throw; // Rethrow so EmailNotificationChannel registers IsSent = false in the DB log
            }
        }
    }
}