using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.BlogPosts.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Queries.GetBlogPostsPaged;

/// <summary>
/// Handler for getting paginated blog posts.
/// </summary>
public sealed class GetBlogPostsPagedQueryHandler : IQueryHandler<GetBlogPostsPagedQuery, PagedResult<BlogPostSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBlogPostsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<BlogPostSummaryDto>>> Handle(
        GetBlogPostsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var pagedPosts = await _unitOfWork.BlogPosts.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.Status,
            request.CategoryId,
            request.AuthorId,
            cancellationToken);

        // Get all unique author and category IDs
        var authorIds = pagedPosts.Items.Select(p => p.AuthorId).Distinct().ToList();
        var categoryIds = pagedPosts.Items.Select(p => p.CategoryId).Distinct().ToList();

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
        var dtos = pagedPosts.Items
            .Select(p => p.ToSummaryDto(
                authors.GetValueOrDefault(p.AuthorId),
                categories.GetValueOrDefault(p.CategoryId)))
            .ToList();

        return Result.Success(new PagedResult<BlogPostSummaryDto>(
            dtos,
            pagedPosts.TotalCount,
            pagedPosts.Page,
            pagedPosts.PageSize));
    }
}
