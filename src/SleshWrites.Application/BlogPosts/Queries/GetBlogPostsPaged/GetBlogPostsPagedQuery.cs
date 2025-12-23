using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Enums;

namespace SleshWrites.Application.BlogPosts.Queries.GetBlogPostsPaged;

/// <summary>
/// Query to get paginated blog posts.
/// </summary>
public sealed record GetBlogPostsPagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    PostStatus? Status = null,
    Guid? CategoryId = null,
    Guid? AuthorId = null) : IQuery<PagedResult<BlogPostSummaryDto>>;
