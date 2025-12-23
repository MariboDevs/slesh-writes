using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Commands.PublishBlogPost;

/// <summary>
/// Handler for publishing a blog post.
/// </summary>
public sealed class PublishBlogPostCommandHandler : ICommandHandler<PublishBlogPostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishBlogPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PublishBlogPostCommand request, CancellationToken cancellationToken)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(request.Id, cancellationToken);
        if (blogPost is null)
            return Result.Failure("Blog post not found.");

        var result = blogPost.Publish();
        if (result.IsFailure)
            return result;

        _unitOfWork.BlogPosts.Update(blogPost);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
