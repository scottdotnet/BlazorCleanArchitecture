using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.User.User;
using BlazorCleanArchitecture.Shared.User.User.Queries;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorCleanArchitecture.Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly Mediator.Mediator Mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(Mediator.Mediator mediator, IHttpContextAccessor httpContextAccessor)
            => (Mediator, _httpContextAccessor) = (mediator, httpContextAccessor);

        public string Domain => _httpContextAccessor.HttpContext!.User.FindFirstValue(JwtRegisteredClaimNames.Website);

        public string Username => _httpContextAccessor.HttpContext!.User.FindFirstValue(JwtRegisteredClaimNames.Email);

        public int UserId => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(JwtRegisteredClaimNames.NameId));

        public async Task<UserDto> User() => await Mediator.Send(new GetCurrentUser { });
    }
}
