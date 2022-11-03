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
    public sealed class ValidateMFACodeHandler : IRequestHandler<ValidateMFACode, bool>
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public ValidateMFACodeHandler(IMemoryCache cache, IConfiguration configuration)
            => (_cache, _configuration) = (cache, configuration);

        public async ValueTask<bool> Handle(ValidateMFACode request, CancellationToken cancellationToken)
        {
            var user = await Task.FromResult(_cache.Get<Domain.User.User>(nameof(ValidateMFACode)));

            var tfa = new TwoFactorAuthenticator();

            return tfa.ValidateTwoFactorPIN(user.MFAKey.ToString(), request.MFACode.ToString());
        }
    }
}
