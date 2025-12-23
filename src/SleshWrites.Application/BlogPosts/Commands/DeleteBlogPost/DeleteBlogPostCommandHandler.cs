using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Commands.DeleteBlogPost;

/// <summary>
/// Handler for deleting a blog post.
/// </summary>
public sealed class DeleteBlogPostCommandHandler : ICommandHandler<DeleteBlogPostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBlogPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBlogPostCommand request, CancellationToken cancellationToken)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(request.Id, cancellationToken);
        if (blogPost is null)
            return Result.Failure("Blog post not found.");

        _unitOfWork.BlogPosts.Remove(blogPost);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
