using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.Authentication.Validators
{
    public sealed class ResetPasswordValidator : AbstractValidator<ResetPassword>
    {
        public ResetPasswordValidator(IMemoryCache cache, IApplicationDbContext context)
        {
            RuleFor(x => x)
                .SetValidator(new Shared.Authentication.Validators.ResetPasswordValidator());

            RuleFor(x => x.ResetId)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await cache.GetOrCreateAsync(nameof(ResetPassword), async e =>
                    {
                        return await context.PasswordResets.AsTracking().Include(x => x.User).SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
                    }) is not null;
                })
                .WithMessage("Reset ID does not exist.");

            RuleFor(x => x.Username)
                .MustAsync(async (username, _) => await Task.FromResult(cache.Get<PasswordReset>(nameof(ResetPassword)).User.Username == username))
                .WithMessage("User doesn't match reset ID.");

            RuleFor(x => x.Password)
                .MustAsync(async (password, _) => await Task.FromResult(cache.Get<PasswordReset>(nameof(ResetPassword)).User.Password != password))
                .WithMessage("Password must not be your previous password.");
        }
    }
}
