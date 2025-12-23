using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Commands.UpdateBlogPost;

/// <summary>
/// Handler for updating an existing blog post.
/// </summary>
public sealed class UpdateBlogPostCommandHandler : ICommandHandler<UpdateBlogPostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBlogPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBlogPostCommand request, CancellationToken cancellationToken)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdWithTagsAsync(request.Id, cancellationToken);
        if (blogPost is null)
            return Result.Failure("Blog post not found.");

        // Update content
        var contentResult = blogPost.UpdateContent(request.Title, request.Content, request.Excerpt);
        if (contentResult.IsFailure)
            return contentResult;

        // Update featured image
        var imageResult = blogPost.SetFeaturedImage(request.FeaturedImage);
        if (imageResult.IsFailure)
            return imageResult;

        // Update category if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
                return Result.Failure("Category not found.");

            var categoryResult = blogPost.UpdateCategory(request.CategoryId.Value);
            if (categoryResult.IsFailure)
                return categoryResult;
        }

        // Update tags if provided
        if (request.TagIds is not null)
        {
            var tags = await _unitOfWork.Tags.GetByIdsAsync(request.TagIds, cancellationToken);
            blogPost.SetTags(tags);
        }

        _unitOfWork.BlogPosts.Update(blogPost);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
