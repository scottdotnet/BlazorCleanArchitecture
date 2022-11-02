using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Tenant;

namespace BlazorCleanArchitecture.Infrastructure.Services
{
    public sealed class TenantService : ITenantService
    {
        public Tenant Tenant { get; set; }
    }
}
