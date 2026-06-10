using System.ComponentModel.DataAnnotations;
using AircraftMRO.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AircraftMRO.Mvc.ViewModels.MaintenanceRecord
{
    public class MaintenanceEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public int WorkOrderId { get; set; }

        [Required]
        public MaintenanceType Type { get; set; }

        [Required]
        public MaintenanceStatus Status { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> WorkOrders { get; set; } = [];
    }
}