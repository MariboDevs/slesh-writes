using FluentValidation;

namespace SleshWrites.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Validator for UpdateTagCommand.
/// </summary>
public sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tag ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");
    }
}
