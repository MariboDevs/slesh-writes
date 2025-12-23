using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.BlogPosts.Queries.GetBlogPostBySlug;

/// <summary>
/// Query to get a blog post by slug.
/// </summary>
public sealed record GetBlogPostBySlugQuery(string Slug) : IQuery<BlogPostDto>;
