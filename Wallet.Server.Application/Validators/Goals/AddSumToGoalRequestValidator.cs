using FluentValidation;
using Wallet.Server.Application.Models.Goals;

namespace Wallet.Server.Application.Validators.Goals;


public class AddSumToGoalRequestValidator : AbstractValidator<AddSumToGoalRequest>
{
    public AddSumToGoalRequestValidator()
    {
        RuleFor(x => x.Sum)
            .GreaterThan(0).WithMessage("Сумма для добавления должна быть больше 0.");
    }
}