using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Infrastructure.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        /// find all class using this Interface IEntityTypeConfiguration, and apply it configuration 
        /// auto-load all entity configurations
        /// Cascading, custom relation , custom set value ...etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }



        public DbSet<Aircraft> Aircrafts => Set<Aircraft>();
        public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<Alert> Alerts => Set<Alert>();

    }
}