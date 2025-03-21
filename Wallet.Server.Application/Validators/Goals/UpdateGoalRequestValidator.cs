using FluentValidation;
using Wallet.Server.Application.Models.Goals;

namespace Wallet.Server.Application.Validators.Goals;

public class UpdateGoalRequestValidator : AbstractValidator<UpdateGoalRequest>
{
    public UpdateGoalRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название цели не может быть пустым.")
            .MinimumLength(3).WithMessage("Название цели должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название цели не может превышать 256 символов.")
            .When(x => x.Name != null);
        RuleFor(x => x.TargetSum)
            .GreaterThan(0).WithMessage("Целевая сумма должна быть больше 0.")
            .When(x => x.TargetSum.HasValue);
    }
}