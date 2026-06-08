using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;

namespace AircraftMRO.Services.Interfaces.Notification
{
    public interface INotificationChannel
    {
        NotificationChannelType ChannelType { get; }
        Task<bool> SendAsync(string userId, string title, string message, string? recipientTarget = null);
    }
}