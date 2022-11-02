using Mediator;
using System.Runtime.Serialization;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    [DataContract]
    public sealed record ResetPassword : IRequest<bool>
    {
        public string Username { get; set; }
        public Guid ResetId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
