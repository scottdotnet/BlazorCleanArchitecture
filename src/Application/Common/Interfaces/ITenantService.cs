using BlazorCleanArchitecture.Domain.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Application.Common.Interfaces
{
    public interface ITenantService
    {
        Tenant Tenant { get; set; }
    }
}
