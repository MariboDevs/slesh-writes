using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Commands.PublishBlogPost;

/// <summary>
/// Command to publish a blog post.
/// </summary>
public sealed record PublishBlogPostCommand(Guid Id) : ICommand;
