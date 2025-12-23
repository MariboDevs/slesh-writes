using FluentValidation;
using SleshWrites.Domain.Constants;

namespace SleshWrites.Application.Categories.Commands.UpdateCategory;

/// <summary>
/// Validator for UpdateCategoryCommand.
/// </summary>
public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(ValidationConstants.Category.NameMaxLength)
            .WithMessage($"Category name must not exceed {ValidationConstants.Category.NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(ValidationConstants.Category.DescriptionMaxLength)
            .WithMessage($"Description must not exceed {ValidationConstants.Category.DescriptionMaxLength} characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).When(x => x.DisplayOrder.HasValue)
            .WithMessage("Display order must be non-negative.");
    }
}
