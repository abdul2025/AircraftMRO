using System.Collections.Concurrent;
using AircraftMRO.Application.DTOs.Emails.Settings;
using AircraftMRO.Application.Interfaces;
using Fluid;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IAppLogger<EmailService> _logger;
        private readonly EmailSettings _settings;
        private static readonly FluidParser _parser = new FluidParser();
        private static readonly ConcurrentDictionary<string, Task<IFluidTemplate>> _templateCache = new();

        public EmailService(IAppLogger<EmailService> logger, IOptions<EmailSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        // Get cached Email template form the time the application start
        private async Task<IFluidTemplate> GetTemplateAsync(string templateName)
        {
            return await _templateCache.GetOrAdd(templateName, async name =>
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", name);

                    var source = await File.ReadAllTextAsync(path);

                    if (!_parser.TryParse(source, out var template, out var error))
                        throw new InvalidOperationException(error);

                    return template;
                });
        }

        public async Task SendAsync(string to, string subject, string templateName, object model)
        {
            try
            {
                var template = await GetTemplateAsync(templateName);

                var context = new TemplateContext(model);
                var html = template.Render(context);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("AircraftMRO", _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = html };

                using var client = new SmtpClient();
                // Bypass certificate validation for internal SMTP relay if needed
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInfo("Email sent successfully to: ", to);
            }
            catch (Exception ex)
            {
                _logger.LogError("Critical error in EmailService while sending to {Recipient}", ex,
                    new { Recipient = to, Template = templateName });
                throw;
            }
        }
    }
}