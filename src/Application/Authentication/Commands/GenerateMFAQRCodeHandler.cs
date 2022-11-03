using BlazorCleanArchitecture.Shared.Authentication.Commands;
using Google.Authenticator;
using Mediator;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Application.Authentication.Commands
{
    public sealed class GenerateMFAQRCodeHandler : IRequestHandler<GenerateMFAQRCode, string>
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public GenerateMFAQRCodeHandler(IMemoryCache cache, IConfiguration configuration)
            => (_cache, _configuration) = (cache, configuration);

        public async ValueTask<string> Handle(GenerateMFAQRCode request, CancellationToken cancellationToken)
        {
            var user = await Task.FromResult(_cache.Get<Domain.User.User>(nameof(GenerateMFAQRCode)));

            var tfa = new TwoFactorAuthenticator();
            var code = tfa.GenerateSetupCode(_configuration.GetSection("MFA")["Issuer"], user.Username, user.MFAKey.ToString(), false);

            return code.QrCodeSetupImageUrl;
        }
    }
}
