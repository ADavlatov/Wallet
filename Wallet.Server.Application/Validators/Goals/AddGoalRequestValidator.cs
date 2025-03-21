using FluentValidation;
using Wallet.Server.Application.Models.Goals;

namespace Wallet.Server.Application.Validators.Goals;

public class AddGoalRequestValidator : AbstractValidator<AddGoalRequest>
{
    public AddGoalRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название цели не может быть пустым.")
            .MinimumLength(3).WithMessage("Название цели должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название цели не может превышать 256 символов.");
        RuleFor(x => x.TargetSum)
            .GreaterThan(0).WithMessage("Целевая сумма должна быть больше 0.");
    }
}
