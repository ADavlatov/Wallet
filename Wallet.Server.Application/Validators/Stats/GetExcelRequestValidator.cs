using FluentValidation;
using Wallet.Server.Application.Models.Stats;

namespace Wallet.Server.Application.Validators.Stats;

public class GetExcelRequestValidator : AbstractValidator<GetExcelRequest>
{
    public GetExcelRequestValidator()
    {
        RuleFor(x => x.Period)
            .NotEmpty().WithMessage("Период не может быть пустым.")
            .Must(BeValidPeriod).WithMessage("Период должен быть одним из следующих значений: week, month, year.");
    }

    private bool BeValidPeriod(string period)
    {
        return period.ToLowerInvariant() == "week" || period.ToLowerInvariant() == "month" || period.ToLowerInvariant() == "year";
    }
}