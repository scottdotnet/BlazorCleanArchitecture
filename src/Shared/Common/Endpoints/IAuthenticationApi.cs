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
    }
}
