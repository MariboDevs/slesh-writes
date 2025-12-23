using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;

/// <summary>
/// Command to create a new blog post.
/// </summary>
public sealed record CreateBlogPostCommand(
    string Title,
    string Content,
    Guid AuthorId,
    Guid CategoryId,
    string? Excerpt = null,
    string? FeaturedImage = null,
    IReadOnlyList<Guid>? TagIds = null) : ICommand<Guid>;
