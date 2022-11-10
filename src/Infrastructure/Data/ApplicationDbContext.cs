using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Common;
using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Common;
using BlazorCleanArchitecture.Infrastructure.Common.EntityFramework;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BlazorCleanArchitecture.Infrastructure.Data
{
    public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantService _tenantService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IEnumerable<EntityTypeConfigurationDependency> _configurations;
        private readonly IEnumerable<IInterceptor> _interceptors;

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
            IEnumerable<EntityTypeConfigurationDependency> configurations,
            IEnumerable<IInterceptor> interceptors
            ) : base(options)
        {
            _tenantService = tenantService;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _configurations = configurations;
            _interceptors = interceptors;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_interceptors);
            optionsBuilder.UseSqlServer(options => options.MigrationsAssembly(InfrastructureAssembly.Assembly.FullName));
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var configuration in _configurations)
                configuration.Configure(modelBuilder);
        }
    }
}
