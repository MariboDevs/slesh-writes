using FluentValidation;

namespace SleshWrites.Application.Tags.Commands.CreateTag;

/// <summary>
/// Validator for CreateTagCommand.
/// </summary>
public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");
    }
}
