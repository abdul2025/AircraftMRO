using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SharedKernel.Logging.Interfaces;

namespace AircraftMRO.Infrastructure.Services
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IAppLogger<NotificationHub> _logger;

        public NotificationHub(IAppLogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            _logger.LogInfo("User linked to real-time socket connection", new { UserId = userId, ConnectionId = Context.ConnectionId });
            await base.OnConnectedAsync();
        }
    }
}