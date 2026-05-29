using AircraftMRO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasOne(a => a.Aircraft)
                .WithMany(a => a.Alerts)
                .HasForeignKey(a => a.AircraftId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}