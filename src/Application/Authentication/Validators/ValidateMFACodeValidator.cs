using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Application.Authentication.Validators
{
    public sealed class ValidateMFACodeValidator : AbstractValidator<ValidateMFACode>
    {
        public ValidateMFACodeValidator(IMemoryCache cache, IApplicationDbContext context)
        {
            RuleFor(x => x.UserId)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await cache.GetOrCreateAsync(nameof(ValidateMFACode), async e =>
                    {
                        return await context.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
                    }) is not null;
                })
                .WithMessage("User does not exist.")
                .MustAsync(async (x, cancellationToken) => await Task.FromResult(cache.Get<Domain.User.User>(nameof(ValidateMFACode)).MFAKey != Guid.Empty))
                .WithMessage("User does not have an MFA Key.")
                .OverridePropertyName("User");
        }
    }
}
