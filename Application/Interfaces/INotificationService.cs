using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Entities;

namespace AircraftMRO.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(Notification notification);
    }
}