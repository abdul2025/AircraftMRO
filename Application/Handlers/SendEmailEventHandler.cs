using AircraftMRO.Application.Events;
using AircraftMRO.Application.Interfaces;
using AircraftMRO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AircraftMRO.Application.Services;
using SharedKernel.Logging.Interfaces;


namespace AircraftMRO.Application.Handlers
{
    public class SendEmailEventHandler : INotificationHandler<SendEmailEvent>
    {
        private readonly IEmailService _emailService; // SMTP service
        private readonly ApplicationDbContext _context;
        private readonly EmailTemplateService _templateService;
        private readonly IAppLogger<SendEmailEventHandler> _logger;

        public SendEmailEventHandler(IEmailService emailService, ApplicationDbContext context, EmailTemplateService templateService, IAppLogger<SendEmailEventHandler> logger)
        {
            _emailService = emailService;
            _context = context;
            _templateService = templateService;
            _logger = logger;
        }

        public async Task Handle(SendEmailEvent ev, CancellationToken ct)
        {
            _logger.LogInfo("Starting SendEmailEvent");
            var emailRecord = await _context.EmailNotifications
                .Include(e => e.Notification)
                .ThenInclude(n => n.Aircraft) // Include the parent the aircraft data
                .FirstOrDefaultAsync(e => e.Id == ev.EmailNotificationId);

            if (emailRecord == null) return;
            _logger.LogInfo($"emailRecord Founded Id: {emailRecord.Id}");
            
            try
            {
                _logger.LogInfo($"Try send Email for {emailRecord.Id}");

                // 1. Get the template name dynamically
                var template = _templateService.GetTemplateName(emailRecord.Notification.Type);

                // 2. Prepare the model dynamically
                var model = _templateService.PrepareModel(emailRecord.Notification);

                // 3. Send using the template and the prepared model
                await _emailService.SendAsync(
                    emailRecord.RecipientEmail,
                    emailRecord.Notification.Title,
                    template,
                    model
                );

                emailRecord.SentAtUtc = DateTime.UtcNow;
                _logger.LogInfo($"Email Sent for {emailRecord.Id}");

            }
            catch (Exception ex)
            {
                _logger.LogError("Try send Email.", ex,
                    new { EventHandler = "SendEmailEventHandler" });
                
                emailRecord.ErrorMessage = ex.Message;
            }

            await _context.SaveChangesAsync();
        }
    }
}