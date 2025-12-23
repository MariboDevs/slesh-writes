using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.ValueObjects;

/// <summary>
/// Represents SEO metadata for a blog post.
/// </summary>
public sealed class PostMetaData : ValueObject
{
    /// <summary>
    /// The SEO title (max 60 characters recommended for search results).
    /// </summary>
    public string? Title { get; }

    /// <summary>
    /// The meta description (max 160 characters recommended for search results).
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Comma-separated keywords for SEO.
    /// </summary>
    public string? Keywords { get; }

    /// <summary>
    /// The canonical URL if different from the default.
    /// </summary>
    public string? CanonicalUrl { get; }

    /// <summary>
    /// Open Graph image URL for social sharing.
    /// </summary>
    public string? OgImage { get; }

    private PostMetaData(string? title, string? description, string? keywords, string? canonicalUrl, string? ogImage)
    {
        Title = title;
        Description = description;
        Keywords = keywords;
        CanonicalUrl = canonicalUrl;
        OgImage = ogImage;
    }

    public static Result<PostMetaData> Create(
        string? title = null,
        string? description = null,
        string? keywords = null,
        string? canonicalUrl = null,
        string? ogImage = null)
    {
        if (title?.Length > 60)
            return Result.Failure<PostMetaData>("SEO title should not exceed 60 characters.");

        if (description?.Length > 160)
            return Result.Failure<PostMetaData>("Meta description should not exceed 160 characters.");

        if (keywords?.Length > 200)
            return Result.Failure<PostMetaData>("Keywords should not exceed 200 characters.");

        if (canonicalUrl is not null && !Uri.TryCreate(canonicalUrl, UriKind.Absolute, out _))
            return Result.Failure<PostMetaData>("Canonical URL must be a valid absolute URL.");

        if (ogImage is not null && !Uri.TryCreate(ogImage, UriKind.Absolute, out _))
            return Result.Failure<PostMetaData>("Open Graph image must be a valid absolute URL.");

        return Result.Success(new PostMetaData(title, description, keywords, canonicalUrl, ogImage));
    }

    public static PostMetaData Empty() => new(null, null, null, null, null);

    public PostMetaData WithTitle(string? title)
    {
        var result = Create(title, Description, Keywords, CanonicalUrl, OgImage);
        return result.IsSuccess ? result.Value : this;
    }

    public PostMetaData WithDescription(string? description)
    {
        var result = Create(Title, description, Keywords, CanonicalUrl, OgImage);
        return result.IsSuccess ? result.Value : this;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Title;
        yield return Description;
        yield return Keywords;
        yield return CanonicalUrl;
        yield return OgImage;
    }
}
