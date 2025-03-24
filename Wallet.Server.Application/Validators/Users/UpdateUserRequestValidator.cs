using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .MinimumLength(3).WithMessage("Имя должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Имя пользователя не может превышать 256 символов.")
            .When(x => !string.IsNullOrEmpty(x.Username));

        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("Пароль должен содержать не менее 8 символов.")
            .MaximumLength(256).WithMessage("Пароль не может превышать 256 символов.")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}