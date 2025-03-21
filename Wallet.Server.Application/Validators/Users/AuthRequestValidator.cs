using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators.Users;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя не может быть пустым.")
            .MinimumLength(3).WithMessage("Имя должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Имя пользователя не может превышать 256 символов.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым.")
            .MinimumLength(8).WithMessage("Пароль должен содержать не менее 8 символов.")
            .MaximumLength(256).WithMessage("Пароль не может превышать 256 символов.");
    }
}