using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Services.Interfaces.Notification
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(
            string userId, string title, string message, 
            IEnumerable<NotificationChannelType> targetChannels, 
            Dictionary<NotificationChannelType, string>? targets = null);
    }
}