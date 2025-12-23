using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.BlogPosts.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Queries.GetRecentBlogPosts;

/// <summary>
/// Handler for getting the most recent published blog posts.
/// </summary>
public sealed class GetRecentBlogPostsQueryHandler : IQueryHandler<GetRecentBlogPostsQuery, IReadOnlyList<BlogPostSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRecentBlogPostsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<BlogPostSummaryDto>>> Handle(
        GetRecentBlogPostsQuery request,
        CancellationToken cancellationToken)
    {
        var posts = await _unitOfWork.BlogPosts.GetRecentPublishedAsync(request.Count, cancellationToken);

        // Get all unique author and category IDs
        var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();
        var categoryIds = posts.Select(p => p.CategoryId).Distinct().ToList();

        // Batch load authors and categories
        var authors = new Dictionary<Guid, string>();
        var categories = new Dictionary<Guid, string>();

        foreach (var authorId in authorIds)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(authorId, cancellationToken);
            if (author is not null)
                authors[authorId] = author.DisplayName;
        }

        foreach (var categoryId in categoryIds)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken);
            if (category is not null)
                categories[categoryId] = category.Name;
        }

        // Map to DTOs
        var dtos = posts
            .Select(p => p.ToSummaryDto(
                authors.GetValueOrDefault(p.AuthorId),
                categories.GetValueOrDefault(p.CategoryId)))
            .ToList();

        return Result.Success<IReadOnlyList<BlogPostSummaryDto>>(dtos);
    }
}
