using FluentValidation;

namespace SleshWrites.Application.BlogPosts.Commands.UpdateBlogPost;

/// <summary>
/// Validator for UpdateBlogPostCommand.
/// </summary>
public sealed class UpdateBlogPostCommandValidator : AbstractValidator<UpdateBlogPostCommand>
{
    public UpdateBlogPostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Blog post ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(500).WithMessage("Excerpt cannot exceed 500 characters.")
            .When(x => x.Excerpt is not null);

        RuleFor(x => x.FeaturedImage)
            .Must(BeAValidUrl).WithMessage("Featured image must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.FeaturedImage));

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID cannot be empty when provided.")
            .When(x => x.CategoryId.HasValue);
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
