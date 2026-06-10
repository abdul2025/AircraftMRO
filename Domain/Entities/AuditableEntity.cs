using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Infrastructure.Identity.Entities;

namespace AircraftMRO.Domain.Entities
{
    public abstract class AuditableEntity
    {
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? UpdatedByUserId { get; set; }
        public ApplicationUser? UpdatedByUser { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }

        public string? DeletedByUserId { get; set; }
        public ApplicationUser? DeletedByUser { get; set; }

        public DateTime? DeletedAtUtc { get; set; }

        public bool IsDeleted { get; set; } 
    }
}