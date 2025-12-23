using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Commands.UpdateBlogPost;

/// <summary>
/// Command to update an existing blog post.
/// </summary>
public sealed record UpdateBlogPostCommand(
    Guid Id,
    string Title,
    string Content,
    string? Excerpt = null,
    string? FeaturedImage = null,
    Guid? CategoryId = null,
    IReadOnlyList<Guid>? TagIds = null) : ICommand;
