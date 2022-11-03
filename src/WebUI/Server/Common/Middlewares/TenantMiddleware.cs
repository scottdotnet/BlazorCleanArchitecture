using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.WebUI.Server.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorCleanArchitecture.WebUI.Server.Common.Middlewares
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

            if (descriptor?.ControllerTypeInfo?.Name == nameof(AuthenticationController))
            {
                _context.Request.EnableBuffering();

                var request = await _context.Request.ReadFromJsonAsync(descriptor.Parameters[0].ParameterType);

                var requestProperty = request.GetType().GetProperties().FirstOrDefault(p => p.Name == "Username" || p.Name == "UserId");

                if (requestProperty?.Name == "Username")
                    context.RequestServices.GetService<ITenantService>().Tenant = await GetTenant(request.GetType().GetProperty(requestProperty.Name).GetValue(request).ToString().Split("@")[1]);

                if (requestProperty?.Name == "UserId")
                {
                    var dbcontext = _context.RequestServices.GetService<IApplicationDbContext>();

                    var id = int.Parse(request.GetType().GetProperty(requestProperty.Name).GetValue(request).ToString());

                    var user = await dbcontext.Users.IgnoreQueryFilters().SingleOrDefaultAsync(u => u.Id == id);

                    if (user?.TenantId is not null)
                        context.RequestServices.GetService<ITenantService>().Tenant = await GetTenant(user.Username.Split("@")[1]);
                }

                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            if (context.Request.Headers.TryGetValue("X-Tenant-Domain", out var tenantDomain))
                context.RequestServices.GetService<ITenantService>().Tenant = await GetTenant(tenantDomain);

            if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
                await HandleAuthenticationToken(authorization);

            await _next(context);
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
