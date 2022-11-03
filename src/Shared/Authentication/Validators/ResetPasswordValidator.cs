using BlazorCleanArchitecture.Shared.Authentication.Commands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BlazorCleanArchitecture.Shared.Authentication.Validators
{
    public sealed class ResetPasswordValidator : AbstractValidator<ResetPassword>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.ResetId)
                .NotEmpty();

            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(320)
                .WithMessage("The length must be no more than 320 characters.")
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("The length must be at least 8 characters.")
                .Must(x => x.Any(c => char.IsWhiteSpace(c)) is false).WithMessage("Password must not contain any white spaces.")
                .Must(x => x.Any(c => char.IsNumber(c))).WithMessage("Password must contain atleast 1 number.")
                .Must(x => new Regex("[^A-z0-9]").Match(x).Success).WithMessage("Password must contain atleast 1 special character.")
                .Must(x => x.Any(c => char.IsUpper(c))).WithMessage("Password must contain atleast 1 upper-case character.")
                .Must(x => x.Any(c => char.IsLower(c))).WithMessage("Password must contain atleast 1 lower-case character.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .Unless(x => string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("Passwords do not match.")
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("The length must be at least 8 characters.")
                .Must(x => x.Any(c => char.IsWhiteSpace(c)) is false).WithMessage("Password must not contain any white spaces.")
                .Must(x => x.Any(c => char.IsNumber(c))).WithMessage("Password must contain atleast 1 number.")
                .Must(x => new Regex("[^A-z0-9]").Match(x).Success).WithMessage("Password must contain atleast 1 special character.")
                .Must(x => x.Any(c => char.IsUpper(c))).WithMessage("Password must contain atleast 1 upper-case character.")
                .Must(x => x.Any(c => char.IsLower(c))).WithMessage("Password must contain atleast 1 lower-case character.");
        }
    }
}
