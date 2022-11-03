using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Shared.Authentication.Validators
{
    public sealed class GenerateMFAQRCodeValidator : AbstractValidator<GenerateMFAQRCode>
    {
        public GenerateMFAQRCodeValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
