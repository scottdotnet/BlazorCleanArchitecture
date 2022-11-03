using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;

namespace BlazorCleanArchitecture.Shared.Authentication.Validators
{
    public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPassword>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(320)
                .WithMessage("The length must be no more than 320 characters.")
                .EmailAddress();
        }
    }
}
