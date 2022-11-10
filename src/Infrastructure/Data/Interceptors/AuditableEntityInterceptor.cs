using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Common;
using BlazorCleanArchitecture.Infrastructure.Services;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Infrastructure.Data.Interceptors
{
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ITenantService _tenantService;

        public AuditableEntityInterceptor(ICurrentUserService currentUserService, IDateTimeService dateTimeService, ITenantService tenantService)
            => (_currentUserService, _dateTimeService, _tenantService) = (currentUserService, dateTimeService, tenantService);

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            foreach (var entry in eventData.Context.ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry)
                {
                    case var e when e.State == EntityState.Added:
                        e.Entity.Created = _dateTimeService.UtcNow;
                        e.Entity.CreatedBy = e.Entity?.CreatedBy ?? _currentUserService.UserId;
                        entry.Property<int>("TenantId").CurrentValue = _tenantService.Tenant.Id;
                        break;

                    case var e when entry.State == EntityState.Modified || entry.HasChangedOwnedEntities():
                        e.Entity.Modified = _dateTimeService.UtcNow;
                        e.Entity.ModifiedBy = _currentUserService.UserId;
                        break;
                }
            }

            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
