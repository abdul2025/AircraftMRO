using AircraftMRO.Infrastructure.Data;
using AircraftMRO.Models.Entities;
using AircraftMRO.Models.Enums;
using AircraftMRO.Services.Interfaces.Notification;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Infrastructure.Services
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEnumerable<INotificationChannel> _channels;
        private readonly ApplicationDbContext _context;
        private readonly IAppLogger<NotificationDispatcher> _logger;

        public NotificationDispatcher(IEnumerable<INotificationChannel> channels, ApplicationDbContext context, IAppLogger<NotificationDispatcher> logger)
        {
            _channels = channels;
            _context = context;
            _logger = logger;
        }

        public async Task DispatchAsync(
                string userId, string title, string message,
                IEnumerable<NotificationChannelType> targetChannels,
                Dictionary<NotificationChannelType, string>? targets = null
            )
        {
            foreach (var channelType in targetChannels)
            {
                var trackingRecord = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Channel = channelType,
                    IsSent = false
                };

                await _context.Notifications.AddAsync(trackingRecord);
                await _context.SaveChangesAsync();

                var channelDriver = _channels.FirstOrDefault(c => c.ChannelType == channelType);
                if (channelDriver == null)
                {
                    trackingRecord.ErrorMessage = $"Driver provider implementation class for '{channelType}' was not found in DI.";
                    await _context.SaveChangesAsync();
                    continue;
                }

                try
                {
                    string? specificRouteTarget = null;

                    if (targets != null)
                    {
                        targets.TryGetValue(channelType, out specificRouteTarget);
                    }

                    bool isSuccessfullyDelivered = await channelDriver.SendAsync(userId, title, message, specificRouteTarget);

                    trackingRecord.IsSent = isSuccessfullyDelivered;
                    if (!isSuccessfullyDelivered)
                    {
                        trackingRecord.ErrorMessage = "External provider gateway error returned.";
                    }
                }
                catch (Exception ex)
                {
                    trackingRecord.IsSent = false;
                    trackingRecord.ErrorMessage = ex.Message;
                    _logger.LogError($"Critical pipeline exception encountered handling channel: {channelType}", ex);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}