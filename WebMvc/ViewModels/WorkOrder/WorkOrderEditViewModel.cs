using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AircraftMRO.Models.ViewModels.WorkOrder
{
    public class WorkOrderEditViewModel
    {
        public int Id { get; set; }

        public int AircraftId { get; set; }

        public string AircraftTailNumber { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public WorkOrderPriority Priority { get; set; }

    }
}