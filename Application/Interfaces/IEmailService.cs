using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftMRO.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string templateName, object model);
    }
}