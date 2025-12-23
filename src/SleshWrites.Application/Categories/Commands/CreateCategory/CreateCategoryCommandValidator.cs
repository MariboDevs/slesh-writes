using FluentValidation;
using SleshWrites.Domain.Constants;

namespace SleshWrites.Application.Categories.Commands.CreateCategory;

/// <summary>
/// Validator for CreateCategoryCommand.
/// </summary>
public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(ValidationConstants.Category.NameMaxLength)
            .WithMessage($"Category name must not exceed {ValidationConstants.Category.NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.Category.DescriptionMaxLength)
            .WithMessage($"Description must not exceed {ValidationConstants.Category.DescriptionMaxLength} characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be non-negative.");
    }
}
