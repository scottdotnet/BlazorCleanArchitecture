using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Tenant;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace BlazorCleanArchitecture.WebUI.Server.Middlewares
{
    public sealed class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private HttpContext _context;

        public TenantMiddleware(RequestDelegate next)
            => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            _context = context;

            var endpoint = context?.GetEndpoint();
            var descriptor = endpoint?.Metadata?
                .OfType<ControllerActionDescriptor>()?
                .FirstOrDefault();

            if (descriptor?.ActionName == nameof(Shared.Authentication.Commands.Login))
                await HandleLogin();

            if (context.Request.Headers.TryGetValue("X-Tenant-Domain", out var tenantDomain))
                context.RequestServices.GetService<ITenantService>().Tenant = await GetTenant(tenantDomain);

            if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
                await HandleAuthenticationToken(authorization);

            await _next(context);
        }

        private async Task HandleLogin()
        {
            var login = await _context.Request.ReadFromJsonAsync<Shared.Authentication.Commands.Login>();
            var domain = login.Username.Split("@")[1];

            var tenant = await GetTenant(domain);

            if (tenant is not null)
                _context.RequestServices.GetService<ITenantService>().Tenant = tenant;
        }

        private async Task HandleAuthenticationToken(string token)
        {
            var domain = new JwtSecurityTokenHandler()
                        .ReadJwtToken(token.Substring("Bearer ".Length).Trim())
                        .Claims
                        .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Website);

            if (domain is not null)
                _context.RequestServices.GetService<ITenantService>().Tenant = await GetTenant(domain.Value);
        }

        private async Task<Tenant> GetTenant(string domain)
        {
            var tenant = await _context.RequestServices.GetService<IApplicationDbContext>().Tenants.SingleOrDefaultAsync(t => t.Domain == domain);

            return tenant;
        }
    }
}
