using FluentValidation;
using Wallet.Server.Application.Models.Transactions;

namespace Wallet.Server.Application.Validators.Transactions;

public class UpdateTransactionRequestValidator : AbstractValidator<UpdateTransactionRequest>
{
    public UpdateTransactionRequestValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("Название транзакции должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название транзакции не может превышать 256 символов.")
            .When(x => x.Name != null);
        RuleFor(x => x.Amount)
            .NotEqual(0).WithMessage("Сумма транзакции не может быть равна 0.")
            .When(x => x.Amount.HasValue);
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Дата транзакции не может быть в будущем.")
            .When(x => x.Date.HasValue);
    }
}