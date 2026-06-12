
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AircraftMRO.Infrastructure.Hubs
{
// You can add [Authorize] to ensure only logged-in users can connect
    [Authorize] 
    public class NotificationHub : Hub
    {
        // Hubs are transient. You don't need to manually manage connections 
        // here; SignalR does it for you.
    }
}