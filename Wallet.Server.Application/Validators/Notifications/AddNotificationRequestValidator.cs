using FluentValidation;
using Wallet.Server.Application.Models.Notifications;

namespace Wallet.Server.Application.Validators.Notifications;

public class AddNotificationRequestValidator : AbstractValidator<AddNotificationRequest>
{
    public AddNotificationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название уведомления не может быть пустым.")
            .MinimumLength(3).WithMessage("Название уведомления должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название уведомления не может превышать 256 символов.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание уведомления не может быть пустым.")
            .MinimumLength(3).WithMessage("Название уведомления должно содержать не менее 3 символов.")
            .MaximumLength(1000).WithMessage("Описание уведомления не может превышать 1000 символов.");
        RuleFor(x => x.DateTime)
            .NotEmpty().WithMessage("Дата и время уведомления не могут быть пустыми.")
            .GreaterThan(DateTime.Now).WithMessage("Дата и время уведомления должны быть в будущем.");
    }
}