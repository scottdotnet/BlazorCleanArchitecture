using Mediator;
using System.Runtime.Serialization;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    [DataContract]
    public sealed record Login : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
