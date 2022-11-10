using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using Google.Authenticator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorCleanArchitecture.Application.Authentication.Validators
{
    public sealed class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator(IApplicationDbContext context)
        {
            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    await context.Users.Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken) is not null)
                .WithMessage("User does not exist.")
                .OverridePropertyName("User");

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    (await context.Users.Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken)).PasswordReset is null)
                .WithMessage("Password reset is currently outstanding, please reset your password before attempting to login.")
                .OverridePropertyName("User");

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    (await context.Users.Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken)).Password == request.Password)
                .WithMessage("Invalid password.")
                .OverridePropertyName("Password");

            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) =>
                    new TwoFactorAuthenticator().ValidateTwoFactorPIN((await context.Users.Include(u => u.PasswordReset).SingleOrDefaultAsync(u => u.Username == request.Username, cancellationToken)).MFAKey.ToString(), request.MFACode))
                .WithMessage("Failed to validate multi-factor authentication.")
                .OverridePropertyName("Multi-factor authentication");
        }
    }
}
