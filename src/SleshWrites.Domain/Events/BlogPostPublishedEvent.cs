using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.Events;

/// <summary>
/// Domain event raised when a blog post is published.
/// </summary>
public sealed class BlogPostPublishedEvent : IDomainEvent
{
    public Guid BlogPostId { get; }
    public string Title { get; }
    public string Slug { get; }
    public Guid AuthorId { get; }
    public DateTime PublishedAt { get; }
    public DateTime OccurredOn { get; }

    public BlogPostPublishedEvent(Guid blogPostId, string title, string slug, Guid authorId, DateTime publishedAt)
    {
        BlogPostId = blogPostId;
        Title = title;
        Slug = slug;
        AuthorId = authorId;
        PublishedAt = publishedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
