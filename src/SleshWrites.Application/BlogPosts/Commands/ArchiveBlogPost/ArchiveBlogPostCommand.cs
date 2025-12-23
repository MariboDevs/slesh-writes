using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Commands.ArchiveBlogPost;

/// <summary>
/// Command to archive a blog post.
/// </summary>
public sealed record ArchiveBlogPostCommand(Guid Id) : ICommand;
