using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Common;
using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Common.EntityFramework;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorCleanArchitecture.Infrastructure.Data
{
    public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IEnumerable<EntityTypeConfigurationDependency> _configurations;

        #region Sets
        #region Tenant
        public DbSet<Tenant> Tenants => Set<Tenant>();
        #endregion

        #region User
        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();
        #endregion
        #endregion

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ITenantService tenantService,
            ICurrentUserService currentUserService,
            IDateTimeService dateTimeService,
            IEnumerable<EntityTypeConfigurationDependency> configurations
            ) : base(options)
        {
            _tenantService = tenantService;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _configurations = configurations;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var configuration in _configurations)
                configuration.Configure(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = entry.Entity?.CreatedBy ?? _currentUserService.UserId;
                        entry.Entity.Created = _dateTimeService.UtcNow;
                        entry.Entity.TenantId = _tenantService.Tenant.Id;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = _currentUserService.UserId;
                        entry.Entity.Modified = _dateTimeService.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
