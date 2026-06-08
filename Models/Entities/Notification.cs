using AircraftMRO.Models.Enums;

namespace AircraftMRO.Models.Entities
{
public class Notification : AuditableEntity
{
    public int Id { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
    
    public NotificationChannelType Channel { get; set; }
    
    public bool IsSent { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public bool IsRead { get; set; }
    
    public DateTime? ReadAtUtc { get; set; }
}
}