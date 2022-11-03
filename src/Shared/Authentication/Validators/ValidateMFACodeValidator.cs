using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.Authentication.Validators
{
    public sealed class ValidateMFACodeValidator : AbstractValidator<ValidateMFACode>
    {
        public ValidateMFACodeValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.MFACode)
                .NotEmpty()
                .ExclusiveBetween(100000, 999999)
                .WithMessage("The length must be exactly 6 characters.");
        }
    }
}
