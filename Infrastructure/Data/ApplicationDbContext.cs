
using AircraftMRO.Infrastructure.Identity.Entities;
using AircraftMRO.Models;
using AircraftMRO.Models.Entities;
using AircraftMRO.Services.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AircraftMRO.Infrastructure.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;

        }

        //*** For any save action AuditableEntity will be created with the associated CRUD entity ***
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();

            return await base.SaveChangesAsync(cancellationToken);
        }
        private void ApplyAuditInformation()
        {
            string? userId = _currentUserService.UserId;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                    entry.Entity.CreatedByUserId = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.CreatedAtUtc).IsModified = false;
                    entry.Property(x => x.CreatedByUserId).IsModified = false;

                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedByUserId = userId;
                }
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = DateTime.UtcNow;
                    entry.Entity.DeletedByUserId = userId;
                }
            }
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