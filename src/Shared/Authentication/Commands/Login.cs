using Mediator;

namespace BlazorCleanArchitecture.Shared.Authentication.Commands
{
    public sealed record Login : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string MFACode { get; set; }
    }
}
