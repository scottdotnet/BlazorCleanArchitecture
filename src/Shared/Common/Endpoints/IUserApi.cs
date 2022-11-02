using BlazorCleanArchitecture.Shared.Common.Interfaces;
using BlazorCleanArchitecture.Shared.User.User;
using Refit;

namespace BlazorCleanArchitecture.Shared.Common.Endpoints
{
    public interface IUserApi : IEndpoint
    {
        [Get("/User")]
        Task<UserDto> GetCurrentUser(CancellationToken cancellationToken);
    }
}
