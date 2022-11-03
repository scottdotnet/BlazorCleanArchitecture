using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Refit;
using System.ServiceModel;

namespace BlazorCleanArchitecture.Shared.Common.Endpoints
{
    [ServiceContract]
    public interface IAuthenticationApi : IEndpoint
    {
        [OperationContract]
        [Post($"/Authentication/{nameof(Authentication.Commands.Login)}")]
        Task<string> Login(Login request, CancellationToken cancellationToken);

        [OperationContract]
        [Post($"/Authentication/{nameof(Authentication.Commands.ResetPassword)}")]
        Task<bool> ResetPassword(ResetPassword request, CancellationToken cancellationToken);

        [OperationContract(IsOneWay = true)]
        [Post($"/Authentication/{nameof(Authentication.Commands.ForgotPassword)}")]
        Task ForgotPassword(ForgotPassword request, CancellationToken cancellationToken);

        [OperationContract]
        [Post($"/Authentication/{nameof(Authentication.Commands.GenerateMFAQRCode)}")]
        Task<string> GenerateMFAQRCode(GenerateMFAQRCode request, CancellationToken cancellationToken);
    }
}
