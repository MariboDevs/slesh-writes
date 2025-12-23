using FluentValidation;
using SleshWrites.Domain.Constants;

namespace SleshWrites.Application.Authors.Commands.UpdateAuthor;

/// <summary>
/// Validator for UpdateAuthorCommand.
/// </summary>
public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Author ID is required.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(ValidationConstants.Author.DisplayNameMaxLength)
            .WithMessage($"Display name must not exceed {ValidationConstants.Author.DisplayNameMaxLength} characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(ValidationConstants.Author.BioMaxLength)
            .WithMessage($"Bio must not exceed {ValidationConstants.Author.BioMaxLength} characters.");

        RuleFor(x => x.AvatarUrl)
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.AvatarUrl))
            .WithMessage("Avatar URL must be a valid URL.");
    }

    private static bool BeAValidUrl(string? url) =>
        string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _);
}
