using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators.Users;

public class RefreshTokensRequestValidator : AbstractValidator<RefreshTokensRequest>
{
    public RefreshTokensRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Токен не может быть пустым.");
    }
}