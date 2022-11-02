using BlazorCleanArchitecture.Shared.User.User;

namespace BlazorCleanArchitecture.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string Domain { get; }
        string Username { get; }
        int UserId { get; }
        Task<UserDto> User();
    }
}
