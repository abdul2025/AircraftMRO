using AircraftMRO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Data.Configurations
{
    public class WorkOrderConfiguration: IEntityTypeConfiguration<MaintenanceRecord>
    {
        public void Configure(EntityTypeBuilder<MaintenanceRecord> builder)
        {
            builder.HasOne(m => m.Aircraft)
                .WithMany(a => a.MaintenanceRecords)
                .HasForeignKey(m => m.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}