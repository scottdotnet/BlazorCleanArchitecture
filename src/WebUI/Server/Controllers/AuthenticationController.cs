using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Endpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorCleanArchitecture.WebUI.Server.Controllers
{
    [AllowAnonymous]
    public sealed class AuthenticationController : ApiControllerBase, IAuthenticationApi
    {
        [HttpPost(nameof(Shared.Authentication.Commands.ForgotPassword))]
        public async Task ForgotPassword(ForgotPassword request, CancellationToken cancellationToken)
            => await Mediator.Send(request, cancellationToken);

        [HttpPost(nameof(Shared.Authentication.Commands.Login))]
        public async Task<string> Login(Login request, CancellationToken cancellationToken)
            => await Mediator.Send(request, cancellationToken);

        [HttpPost(nameof(Shared.Authentication.Commands.ResetPassword))]
        public async Task<bool> ResetPassword(ResetPassword request, CancellationToken cancellationToken)
            => await Mediator.Send(request, cancellationToken);
    }
}
