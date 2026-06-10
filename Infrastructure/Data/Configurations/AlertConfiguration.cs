using AircraftMRO.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.HasOne(a => a.Aircraft)
                .WithMany(a => a.Alerts)
                .HasForeignKey(a => a.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}