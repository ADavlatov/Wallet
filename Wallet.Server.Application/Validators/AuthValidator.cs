using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators;

public class AuthValidator : AbstractValidator<AuthRequest>
{
    public AuthValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(5).WithMessage("Username minimum length is 5")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password minimum length is 6")
            .MaximumLength(30).WithMessage("Password must not exceed 30 characters");
    }
}