using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Interfaces;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BlazorCleanArchitecture.Application.Authentication.Commands
{
    public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPassword>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeService _dateTimeService;

        public ForgotPasswordHandler(IApplicationDbContext context, IDateTimeService dateTimeService)
            => (_context, _dateTimeService) = (context, dateTimeService);

        public async ValueTask<Unit> Handle(ForgotPassword request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user is null)
                return Unit.Value;

            var reset = new PasswordReset { Id = Guid.NewGuid(), UserId = user.Id, Date = _dateTimeService.UtcNow };

            await _context.PasswordResets.AddAsync(reset);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
