using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Queries.GetRecentBlogPosts;

/// <summary>
/// Query to get the most recent published blog posts.
/// </summary>
public sealed record GetRecentBlogPostsQuery(int Count = 5) : IQuery<IReadOnlyList<BlogPostSummaryDto>>;
