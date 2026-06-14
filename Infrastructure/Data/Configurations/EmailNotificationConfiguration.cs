using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AircraftMRO.Infrastructure.Data.Configurations
{
    public class EmailNotificationConfiguration: IEntityTypeConfiguration<EmailNotification>
    {
        public void Configure(EntityTypeBuilder<EmailNotification> builder)
        {
            builder.ToTable("EmailNotifications");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RecipientEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.ErrorMessage)
                .HasMaxLength(2000);

            // Indexing for quick lookups during background processing
            builder.HasIndex(e => e.SentAtUtc);
        }
    }
}