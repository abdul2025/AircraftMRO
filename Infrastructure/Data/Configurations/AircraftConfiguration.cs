using AircraftMRO.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    public class AircraftConfiguration
        : IEntityTypeConfiguration<Aircraft>
    {
        public void Configure(EntityTypeBuilder<Aircraft> builder)
        {
            builder
                .HasIndex(a => a.TailNumber)
                .IsUnique();
        }
    }
}