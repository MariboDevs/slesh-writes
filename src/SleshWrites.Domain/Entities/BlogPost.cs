using SleshWrites.Domain.Common;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Events;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Domain.Entities;

/// <summary>
/// Represents a blog post - the main aggregate root of the domain.
/// </summary>
public sealed class BlogPost : Entity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public string? Excerpt { get; private set; }
    public string? FeaturedImage { get; private set; }
    public PostStatus Status { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid CategoryId { get; private set; }
    public PostMetaData MetaData { get; private set; } = null!;

    // Navigation property - loaded by EF Core
    public Category? Category { get; private set; }

    private readonly List<Tag> _tags = [];
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private BlogPost() { }

    private BlogPost(
        string title,
        Slug slug,
        string content,
        string? excerpt,
        Guid authorId,
        Guid categoryId)
    {
        Title = title;
        Slug = slug;
        Content = content;
        Excerpt = excerpt;
        AuthorId = authorId;
        CategoryId = categoryId;
        Status = PostStatus.Draft;
        MetaData = PostMetaData.Empty();
    }

    /// <summary>
    /// Creates a new blog post in draft status.
    /// </summary>
    public static Result<BlogPost> Create(
        string title,
        string content,
        Guid authorId,
        Guid categoryId,
        string? excerpt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<BlogPost>("Title cannot be empty.");

        if (title.Length > 200)
            return Result.Failure<BlogPost>("Title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<BlogPost>("Content cannot be empty.");

        if (content.Length < 50)
            return Result.Failure<BlogPost>("Content must be at least 50 characters.");

        if (authorId == Guid.Empty)
            return Result.Failure<BlogPost>("Author ID cannot be empty.");

        if (categoryId == Guid.Empty)
            return Result.Failure<BlogPost>("Category ID cannot be empty.");

        var slugResult = Slug.Create(title);
        if (slugResult.IsFailure)
            return Result.Failure<BlogPost>($"Failed to create slug: {slugResult.Error}");

        var truncatedExcerpt = excerpt?.Length > 500 ? excerpt[..500] : excerpt;

        var post = new BlogPost(title, slugResult.Value, content, truncatedExcerpt, authorId, categoryId);
        post.AddDomainEvent(new BlogPostCreatedEvent(post.Id, title, authorId));

        return Result.Success(post);
    }

    /// <summary>
    /// Updates the blog post content.
    /// </summary>
    public Result UpdateContent(string title, string content, string? excerpt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure("Title cannot be empty.");

        if (title.Length > 200)
            return Result.Failure("Title cannot exceed 200 characters.");

        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure("Content cannot be empty.");

        if (content.Length < 50)
            return Result.Failure("Content must be at least 50 characters.");

        // Only update slug if title changed and post is not published
        if (Title != title && Status == PostStatus.Draft)
        {
            var slugResult = Slug.Create(title);
            if (slugResult.IsFailure)
                return Result.Failure($"Failed to create slug: {slugResult.Error}");

            Slug = slugResult.Value;
        }

        Title = title;
        Content = content;
        Excerpt = excerpt?.Length > 500 ? excerpt[..500] : excerpt;
        SetUpdatedAt();

        return Result.Success();
    }

    /// <summary>
    /// Publishes the blog post, making it visible to readers.
    /// </summary>
    public Result Publish()
    {
        if (Status == PostStatus.Published)
            return Result.Failure("Blog post is already published.");

        if (Status == PostStatus.Archived)
            return Result.Failure("Cannot publish an archived blog post. Unarchive it first.");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        SetUpdatedAt();

        AddDomainEvent(new BlogPostPublishedEvent(Id, Title, Slug.Value, AuthorId, PublishedAt.Value));

        return Result.Success();
    }

    /// <summary>
    /// Archives the blog post, hiding it from readers.
    /// </summary>
    public Result Archive()
    {
        if (Status == PostStatus.Archived)
            return Result.Failure("Blog post is already archived.");

        Status = PostStatus.Archived;
        SetUpdatedAt();

        return Result.Success();
    }

    /// <summary>
    /// Unarchives a previously archived blog post, returning it to draft status.
    /// </summary>
    public Result Unarchive()
    {
        if (Status != PostStatus.Archived)
            return Result.Failure("Blog post is not archived.");

        Status = PostStatus.Draft;
        SetUpdatedAt();

        return Result.Success();
    }

    /// <summary>
    /// Updates the category of the blog post.
    /// </summary>
    public void UpdateCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return;

        CategoryId = categoryId;
        SetUpdatedAt();
    }

    /// <summary>
    /// Sets the featured image URL.
    /// </summary>
    public Result SetFeaturedImage(string? imageUrl)
    {
        if (imageUrl is not null && !Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
            return Result.Failure("Featured image must be a valid absolute URL.");

        FeaturedImage = imageUrl;
        SetUpdatedAt();

        return Result.Success();
    }

    /// <summary>
    /// Updates the SEO metadata.
    /// </summary>
    public Result UpdateMetaData(PostMetaData metaData)
    {
        MetaData = metaData;
        SetUpdatedAt();
        return Result.Success();
    }

    /// <summary>
    /// Adds a tag to the blog post.
    /// </summary>
    public void AddTag(Tag tag)
    {
        if (_tags.Any(t => t.Id == tag.Id))
            return;

        _tags.Add(tag);
        SetUpdatedAt();
    }

    /// <summary>
    /// Removes a tag from the blog post.
    /// </summary>
    public void RemoveTag(Guid tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null)
        {
            _tags.Remove(tag);
            SetUpdatedAt();
        }
    }

    /// <summary>
    /// Clears all tags from the blog post.
    /// </summary>
    public void ClearTags()
    {
        if (_tags.Count > 0)
        {
            _tags.Clear();
            SetUpdatedAt();
        }
    }

    /// <summary>
    /// Replaces all tags with a new set.
    /// </summary>
    public void SetTags(IEnumerable<Tag> tags)
    {
        _tags.Clear();
        _tags.AddRange(tags.DistinctBy(t => t.Id));
        SetUpdatedAt();
    }
}
