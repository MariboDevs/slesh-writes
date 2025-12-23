using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Commands.DeleteBlogPost;

/// <summary>
/// Command to delete a blog post.
/// </summary>
public sealed record DeleteBlogPostCommand(Guid Id) : ICommand;
