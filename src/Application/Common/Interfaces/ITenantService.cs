using BlazorCleanArchitecture.Domain.Tenant;

namespace BlazorCleanArchitecture.Application.Common.Interfaces
{
    public interface ITenantService
    {
        Tenant Tenant { get; set; }
    }
}
