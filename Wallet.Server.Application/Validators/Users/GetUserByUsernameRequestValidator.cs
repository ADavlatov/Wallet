using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators.Users;

public class GetUserByUsernameRequestValidator : AbstractValidator<GetUserByUsernameRequest>
{
    public GetUserByUsernameRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя не может быть пустым.")
            .MinimumLength(3).WithMessage("Имя должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Имя пользователя не может превышать 256 символов.");
    }
}