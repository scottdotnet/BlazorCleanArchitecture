using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using Mediator;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.Authentication.Commands
{
    public sealed class ResetPasswordHandler : IRequestHandler<ResetPassword, bool>
    {
        private readonly IMemoryCache _cache;
        private readonly IApplicationDbContext _context;

        public ResetPasswordHandler(IMemoryCache cache, IApplicationDbContext context)
            => (_cache, _context) = (cache, context);

        public async ValueTask<bool> Handle(ResetPassword request, CancellationToken cancellationToken)
        {
            var reset = _cache.Get<PasswordReset>(nameof(ResetPassword));

            reset.User.Password = request.Password;

            _context.PasswordResets.Remove(reset);

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
