namespace SleshWrites.API.Contracts.BlogPosts;

/// <summary>
/// Request model for creating a new blog post.
/// </summary>
public sealed record CreateBlogPostRequest(
    string Title,
    string Content,
    Guid AuthorId,
    Guid CategoryId,
    string? Excerpt,
    string? FeaturedImage,
    IReadOnlyList<Guid>? TagIds);
