using SleshWrites.Application.BlogPosts.DTOs;
using SleshWrites.Application.BlogPosts.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Queries.GetBlogPostById;

/// <summary>
/// Handler for getting a blog post by ID.
/// </summary>
public sealed class GetBlogPostByIdQueryHandler : IQueryHandler<GetBlogPostByIdQuery, BlogPostDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBlogPostByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BlogPostDto>> Handle(GetBlogPostByIdQuery request, CancellationToken cancellationToken)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdWithTagsAsync(request.Id, cancellationToken);
        if (blogPost is null)
            return Result.Failure<BlogPostDto>("Blog post not found.");

        // Get author and category names
        var author = await _unitOfWork.Authors.GetByIdAsync(blogPost.AuthorId, cancellationToken);
        var category = await _unitOfWork.Categories.GetByIdAsync(blogPost.CategoryId, cancellationToken);

        return Result.Success(blogPost.ToDto(author?.DisplayName, category?.Name));
    }
}
