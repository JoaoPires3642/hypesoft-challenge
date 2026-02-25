using FluentValidation;
using Hypesoft.Application.Commands;

namespace Hypesoft.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade em estoque não pode ser negativa.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("A categoria é obrigatória.");
    }
}