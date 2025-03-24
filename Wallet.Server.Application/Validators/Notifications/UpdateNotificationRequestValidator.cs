using FluentValidation;
using Wallet.Server.Application.Models.Notifications;

namespace Wallet.Server.Application.Validators.Notifications;

public class UpdateNotificationRequestValidator : AbstractValidator<UpdateNotificationRequest>
{
    public UpdateNotificationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название уведомления не может быть пустым.")
            .MinimumLength(3).WithMessage("Название уведомления должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название уведомления не может превышать 256 символов.")
            .When(x => x.Name != null);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Описание уведомления не может быть пустым.")
            .MinimumLength(3).WithMessage("Название уведомления должно содержать не менее 3 символов.")
            .MaximumLength(1000).WithMessage("Описание уведомления не может превышать 1000 символов.")
            .When(x => x.Description != null);
        RuleFor(x => x.DateTime)
            .GreaterThan(DateTime.Now).WithMessage("Дата и время уведомления должны быть в будущем.")
            .When(x => x.DateTime.HasValue);
    }
}