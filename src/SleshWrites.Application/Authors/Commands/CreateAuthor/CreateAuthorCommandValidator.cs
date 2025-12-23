using FluentValidation;

namespace SleshWrites.Application.Authors.Commands.CreateAuthor;

/// <summary>
/// Validator for CreateAuthorCommand.
/// </summary>
public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.AzureAdB2CId)
            .NotEmpty().WithMessage("Azure AD B2C ID is required.")
            .MaximumLength(128).WithMessage("Azure AD B2C ID must not exceed 128 characters.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
    }
}
