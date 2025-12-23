using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;

/// <summary>
/// Handler for creating a new blog post.
/// </summary>
public sealed class CreateBlogPostCommandHandler : ICommandHandler<CreateBlogPostCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBlogPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBlogPostCommand request, CancellationToken cancellationToken)
    {
        // Verify author exists
        var author = await _unitOfWork.Authors.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author is null)
            return Result.Failure<Guid>("Author not found.");

        // Verify category exists
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<Guid>("Category not found.");

        // Create the blog post
        var blogPostResult = BlogPost.Create(
            request.Title,
            request.Content,
            request.AuthorId,
            request.CategoryId,
            request.Excerpt);

        if (blogPostResult.IsFailure)
            return Result.Failure<Guid>(blogPostResult.Error!);

        var blogPost = blogPostResult.Value;

        // Set featured image if provided
        if (!string.IsNullOrWhiteSpace(request.FeaturedImage))
        {
            var imageResult = blogPost.SetFeaturedImage(request.FeaturedImage);
            if (imageResult.IsFailure)
                return Result.Failure<Guid>(imageResult.Error!);
        }

        // Add tags if provided
        if (request.TagIds?.Count > 0)
        {
            var tags = await _unitOfWork.Tags.GetByIdsAsync(request.TagIds, cancellationToken);
            blogPost.SetTags(tags);
        }

        _unitOfWork.BlogPosts.Add(blogPost);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(blogPost.Id);
    }
}
