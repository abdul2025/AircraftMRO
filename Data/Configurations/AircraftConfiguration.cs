using AircraftMRO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Data.Configurations
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