using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using Google.Authenticator;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorCleanArchitecture.Application.Authentication.Commands
{
    public sealed class LoginHandler : IRequestHandler<Login, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginHandler(IApplicationDbContext context, IConfiguration configuration)
            => (_context, _configuration) = (context, configuration);

        public async ValueTask<string> Handle(Login request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                //new Claim(JwtRegisteredClaimNames.Website, _httpContextAccessor.HttpContext!.Request.Headers["X-Tenant-Domain"]),
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:Timeout"])), signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
