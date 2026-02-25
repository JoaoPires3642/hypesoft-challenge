using FluentValidation;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MinimumLength(3).WithMessage("O nome da categoria deve ter pelo menos 3 caracteres.");
    }
}