using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Queries.GetBlogPostById;

/// <summary>
/// Query to get a blog post by ID.
/// </summary>
public sealed record GetBlogPostByIdQuery(Guid Id) : IQuery<BlogPostDto>;
