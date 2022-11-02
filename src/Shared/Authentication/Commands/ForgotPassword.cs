using Mediator;
using System.Runtime.Serialization;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    [DataContract]
    public sealed record ForgotPassword : IRequest
    {
        public string Username { get; set; }
    }
}
