using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.User.User.Queries;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.User.User.Validators
{
    public sealed class GetCurrentUserValidator : AbstractValidator<GetCurrentUser>
    {
        public GetCurrentUserValidator(ICurrentUserService currentUserService, IApplicationDbContext context, IMemoryCache cache)
        {
            RuleFor(x => x)
                .MustAsync(async (x, cancellationToken) =>
                {
                    return await cache.GetOrCreateAsync(nameof(GetCurrentUser), async e =>
                    {
                        return await context.Users.SingleOrDefaultAsync(u => u.Username == currentUserService.Username, cancellationToken);
                    }) is not null;
                })
                .OverridePropertyName("User")
                .WithMessage("User does not exist.");
        }
    }
}
