using AircraftMRO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    public class MaintenanceRecordConfiguration : IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.HasOne(m => m.WorkOrder)
                .WithMany(w => w.MaintenanceRecords)
                .HasForeignKey(m => m.WorkOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}