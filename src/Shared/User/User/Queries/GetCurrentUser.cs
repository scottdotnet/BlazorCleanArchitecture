using Mediator;
using System.Runtime.Serialization;

namespace BlazorCleanArchitecture.Shared.User.User.Queries
{
    [DataContract]
    public sealed record GetCurrentUser : IRequest<UserDto>
    {
    }
}
