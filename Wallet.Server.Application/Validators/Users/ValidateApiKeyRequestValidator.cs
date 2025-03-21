using FluentValidation;
using Wallet.Server.Application.Models.Users;

namespace Wallet.Server.Application.Validators.Users;

public class ValidateApiKeyRequestValidator : AbstractValidator<ValidateApiKeyRequest>
{
    public ValidateApiKeyRequestValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty().WithMessage("Ключ API не может быть пустым.");

        RuleFor(x => x.TelegramUserId)
            .NotEmpty().WithMessage("Идентификатор пользователя Telegram не может быть пустым.");
    }
}