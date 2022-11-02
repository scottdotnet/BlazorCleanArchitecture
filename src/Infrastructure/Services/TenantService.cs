using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Tenant;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Infrastructure.Services
{
    public sealed class TenantService : ITenantService
    {
        public Tenant Tenant { get; set; }
    }
}
