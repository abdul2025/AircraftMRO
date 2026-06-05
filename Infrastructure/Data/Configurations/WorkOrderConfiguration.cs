using AircraftMRO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasOne(w => w.Aircraft)
                .WithMany(a => a.WorkOrders)
                .HasForeignKey(w => w.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}