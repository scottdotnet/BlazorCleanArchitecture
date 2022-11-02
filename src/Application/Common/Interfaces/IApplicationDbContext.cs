using BlazorCleanArchitecture.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace BlazorCleanArchitecture.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Tenant.Tenant> Tenants { get; }
        DbSet<Domain.User.User> Users { get; }
        DbSet<PasswordReset> PasswordResets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
