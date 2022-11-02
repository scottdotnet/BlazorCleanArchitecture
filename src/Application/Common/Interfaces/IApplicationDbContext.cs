using Microsoft.EntityFrameworkCore;

namespace BlazorCleanArchitecture.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Tenant.Tenant> Tenants { get; }
        DbSet<Domain.User.User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
