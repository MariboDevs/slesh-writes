namespace SleshWrites.Domain.Enums;

/// <summary>
/// Represents the publication status of a blog post.
/// </summary>
public enum PostStatus
{
    /// <summary>
    /// Post is being written and is not visible to readers.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Post is published and visible to readers.
    /// </summary>
    Published = 1,

    /// <summary>
    /// Post was published but has been archived and is no longer visible.
    /// </summary>
    Archived = 2
}
