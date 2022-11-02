using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.Authentication.Validators
{
    public sealed class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator(IMemoryCache cache, IApplicationDbContext context)
        {
            RuleFor(x => x.Username)
                .MustAsync(async (username, cancellationToken) =>
                {
                    return await cache.GetOrCreateAsync(nameof(Login), async e =>
                    {
                        return await context.Users.AsNoTracking().Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == username, cancellationToken);
                    }) is not null;
                })
                .WithMessage("User does not exist.")
                .MustAsync(async (username, cancellationToken) => await Task.FromResult(cache.Get<Domain.User.User>(nameof(Login)).PasswordReset is null))
                .WithMessage("Password reset is currently outstanding, please reset your password before attempting to login.");

            RuleFor(x => x.Password)
                .MustAsync(async (x, cancellationToken) => await Task.FromResult(cache.Get<Domain.User.User>(nameof(Login)).Password == x))
                .WithMessage("Invalid password.");
        }
    }
}
