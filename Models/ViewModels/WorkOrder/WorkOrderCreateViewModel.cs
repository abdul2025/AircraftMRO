using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AircraftMRO.Models.ViewModels.WorkOrder
{
    public class WorkOrderCreateViewModel
    {
        [Required]
        public int AircraftId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Medium;

        public List<SelectListItem> Aircrafts { get; set; } = [];
    }
}