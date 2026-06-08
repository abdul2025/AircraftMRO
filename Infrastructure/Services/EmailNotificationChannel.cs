using System.Net;
using System.Net.Mail;
using AircraftMRO.Models.Enums;
using AircraftMRO.Services.Interfaces.INotification;
using AircraftMRO.Services.Interfaces.Notification;
using Microsoft.Extensions.Configuration;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Infrastructure.Services
{
    public class EmailNotificationChannel : INotificationChannel
    {
        public NotificationChannelType ChannelType => NotificationChannelType.Email;
        private readonly IEmailService _emailService;

        public EmailNotificationChannel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> SendAsync(string userId, string title, string message, string? recipientTarget = null)
        {
            if (string.IsNullOrEmpty(recipientTarget)) return false;

            try
            {
                await _emailService.SendEmailAsync(recipientTarget, title, message);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}