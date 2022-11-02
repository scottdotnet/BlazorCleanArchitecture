using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.User.User.Queries
{
    [DataContract]
    public sealed record GetCurrentUser : IRequest<UserDto>
    {
    }
}
