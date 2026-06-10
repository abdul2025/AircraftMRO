using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    // *** this is to ensure AuditableEntity that reference three fields to the same table ApplicationUser, need to explicit identifying the each field reference to ApplicationUser ****
    public static class AuditableEntityConfiguration
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            var auditableTypes = modelBuilder.Model
                .GetEntityTypes()
                .Where(t => typeof(AuditableEntity)
                    .IsAssignableFrom(t.ClrType));

            foreach (var entityType in auditableTypes)
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.CreatedByUser))
                    .WithMany()
                    .HasForeignKey(nameof(AuditableEntity.CreatedByUserId))
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.UpdatedByUser))
                    .WithMany()
                    .HasForeignKey(nameof(AuditableEntity.UpdatedByUserId))
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(ApplicationUser), nameof(AuditableEntity.DeletedByUser))
                    .WithMany()
                    .HasForeignKey(nameof(AuditableEntity.DeletedByUserId))
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(AuditableEntity.CreatedAtUtc))
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(AuditableEntity.IsDeleted))
                    .HasDefaultValue(false);
            }
        }
    }
}