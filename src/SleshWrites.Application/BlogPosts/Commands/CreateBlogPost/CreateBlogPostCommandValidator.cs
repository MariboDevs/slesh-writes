using FluentValidation;

namespace SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;

/// <summary>
/// Validator for CreateBlogPostCommand.
/// </summary>
public sealed class CreateBlogPostCommandValidator : AbstractValidator<CreateBlogPostCommand>
{
    public CreateBlogPostCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Excerpt)
            .MaximumLength(500).WithMessage("Excerpt cannot exceed 500 characters.")
            .When(x => x.Excerpt is not null);

        RuleFor(x => x.FeaturedImage)
            .Must(BeAValidUrl).WithMessage("Featured image must be a valid URL.")
            .When(x => !string.IsNullOrWhiteSpace(x.FeaturedImage));
    }

    private static bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
