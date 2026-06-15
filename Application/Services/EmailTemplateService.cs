
using System.Text.Json;
using AircraftMRO.Application.DTOs.EmailTemplates;
using AircraftMRO.Domain.Entities;
using AircraftMRO.Domain.Enums;

namespace AircraftMRO.Application.Services
{
    public class EmailTemplateService
    {
        // Maping by NotificationType to Select Email templates
        public string GetTemplateName(NotificationType type) => type switch
        {
            NotificationType.OverdueWorkOrder => "OverdueAlert.liquid",
            NotificationType.AircraftGrounded => "GroundedAlert.liquid",
            _ => "GeneralAlert.liquid"
        };

        // Maping by NotificationType to PrepareModel for Email templates
        public object PrepareModel(Notification notification)
        {
            try
            {
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // This maps "aircraftId" to "AircraftId"
                };

                return notification.Type switch
                {
                    NotificationType.OverdueWorkOrder =>
                        JsonSerializer.Deserialize<OverdueWorkOrderEmailModel>(notification.DataPayload ?? "{}", options)
                        ?? new OverdueWorkOrderEmailModel(),

                    NotificationType.AircraftGrounded =>
                        JsonSerializer.Deserialize<GroundedAircraftModel>(notification.DataPayload ?? "{}", options)
                        ?? new GroundedAircraftModel(),

                    _ => new { Message = notification.Message }
                };
            }catch
            {
                Console.WriteLine("Error JSON Loading DataPayload");
                return new { Message = notification.Message };
            }
        }
    }
}