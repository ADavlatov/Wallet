using FluentValidation;
using Wallet.Server.Application.Models.Categories;

namespace Wallet.Server.Application.Validators.Categories;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории не может быть пустым.")
            .MinimumLength(3).WithMessage("Название категории должно содержать не менее 3 символов.")
            .MaximumLength(256).WithMessage("Название категории не может превышать 256 символов.")
            .When(x => x.Name != null);
    }
}