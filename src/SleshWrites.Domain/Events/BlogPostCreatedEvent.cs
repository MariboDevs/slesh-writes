using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.Events;

/// <summary>
/// Domain event raised when a new blog post is created.
/// </summary>
public sealed class BlogPostCreatedEvent : IDomainEvent
{
    public Guid BlogPostId { get; }
    public string Title { get; }
    public Guid AuthorId { get; }
    public DateTime OccurredOn { get; }

    public BlogPostCreatedEvent(Guid blogPostId, string title, Guid authorId)
    {
        BlogPostId = blogPostId;
        Title = title;
        AuthorId = authorId;
        OccurredOn = DateTime.UtcNow;
    }
}
