
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;
using AircraftMRO.Infrastructure.Data;
using MediatR;
using SharedKernel.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using AircraftMRO.Application.Events;
using Microsoft.AspNetCore.Identity;
using AircraftMRO.Infrastructure.Identity.Entities;

namespace AircraftMRO.Infrastructure.BackgroundJobs
{
    public class ProcessEmailNotifc
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<AlertJobsService> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;



        public ProcessEmailNotifc(ApplicationDbContext context, IAppLogger<AlertJobsService> logger, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task RunEmailProcessor()
        {
            try
            {
                _logger.LogInfo("Starting ProcessEmailNotifc background job.");

                await CheckNewEmailNotification();

                _logger.LogInfo("ProcessEmailNotifc background job completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("ProcessEmailNotifc background job failed.",
                    ex,
                    new { JobName = "ProcessEmailNotifc BackGround Job" });
                throw;
            }
        }


        public async Task<List<string>> GetAdminEmails()
        {
            // Retrieve users in the "Admin" role
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            // Select just their emails
            return adminUsers.Select(u => u.Email!).ToList();
        }


        public async Task CheckNewEmailNotification()
        {
            // Find all notifications that need email processing
            var pending = await _context.Notifications
                .Where(n => (n.Channel == NotificationChannel.Email || n.Channel == NotificationChannel.Both) && !n.IsEmailProcessed)
                .OrderBy(n => n.Id)
                .Take(200) // Takes only 200 Notification
                .ToListAsync();

            if (!pending.Any()) return; // Early exit if nothing to do

            _logger.LogInfo($"Starting email processing job. Pending: {pending.Count}");

            // Set up a batch saving to avoid saving in the loop 
            // and making sure event not trigger before it been saved the IsEmailProcessed and New EmailNotification

            int batchSize = 50;
            var newEmailNotifications = new List<EmailNotification>();

            // var adminEmails = await GetAdminEmails();

            // TODO: Enhance to sent to Admin Email or any required Recipient

            foreach (var notification in pending)
            {
                var emailRecord = new EmailNotification
                {
                    NotificationId = notification.Id,
                    RecipientEmail = "abdul2021alsh@gmail.com",
                    SentAtUtc = null
                };

                _context.EmailNotifications.Add(emailRecord);
                notification.IsEmailProcessed = true;
                newEmailNotifications.Add(emailRecord);

                // Batch processing
                if (newEmailNotifications.Count >= batchSize)
                {
                    await SaveAndPublishBatch(newEmailNotifications);
                    newEmailNotifications.Clear();
                }
            }

            // This saves any remaining items that didn't hit the batchSize limit
            if (newEmailNotifications.Any())
            {
                await SaveAndPublishBatch(newEmailNotifications);
            }
        }

        // Helper method to keep code clean
        private async Task SaveAndPublishBatch(List<EmailNotification> batch)
        {
            await _context.SaveChangesAsync();
            foreach (var newEmail in batch)
            {
                await _mediator.Publish(new SendEmailEvent { EmailNotificationId = newEmail.Id });
            }
        }

    }


}