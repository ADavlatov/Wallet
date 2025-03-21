using FluentValidation;
using Wallet.Server.Application.Models.Transactions;

namespace Wallet.Server.Application.Validators.Transactions;

public class AddTransactionRequestValidator : AbstractValidator<AddTransactionRequest>
{
    public AddTransactionRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Название транзакции должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название транзакции не может превышать 256 символов.");
        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("Сумма транзакции не может быть равна 0.");
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Дата транзакции не может быть пустой.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Дата транзакции не может быть в будущем.");
    }
}