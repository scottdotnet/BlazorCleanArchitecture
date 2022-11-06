using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Refit;
using System.ServiceModel;

namespace BlazorCleanArchitecture.Shared.Common.Endpoints
{
    public interface IAuthenticationApi : IEndpoint
    {
        [Post($"/Authentication/{nameof(Authentication.Commands.Login)}")]
        Task<string> Login(Login request, CancellationToken cancellationToken);

        [Post($"/Authentication/{nameof(Authentication.Commands.ResetPassword)}")]
        Task<bool> ResetPassword(ResetPassword request, CancellationToken cancellationToken);

        [Post($"/Authentication/{nameof(Authentication.Commands.ForgotPassword)}")]
        Task ForgotPassword(ForgotPassword request, CancellationToken cancellationToken);

        [Post($"/Authentication/{nameof(Authentication.Commands.GenerateMFAQRCode)}")]
        Task<string> GenerateMFAQRCode(GenerateMFAQRCode request, CancellationToken cancellationToken);

        [Post($"/Authentication/{nameof(Authentication.Commands.ValidateMFACode)}")]
        Task<bool> ValidateMFACode(ValidateMFACode request, CancellationToken cancellationToken);
    }
}
