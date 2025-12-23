namespace SleshWrites.API.Contracts.BlogPosts;

/// <summary>
/// Request model for updating an existing blog post.
/// </summary>
public sealed record UpdateBlogPostRequest(
    string Title,
    string Content,
    string? Excerpt,
    string? FeaturedImage,
    Guid? CategoryId,
    IReadOnlyList<Guid>? TagIds);
