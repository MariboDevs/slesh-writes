using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.BlogPosts.Commands.ArchiveBlogPost;

/// <summary>
/// Handler for archiving a blog post.
/// </summary>
public sealed class ArchiveBlogPostCommandHandler : ICommandHandler<ArchiveBlogPostCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveBlogPostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ArchiveBlogPostCommand request, CancellationToken cancellationToken)
    {
        var blogPost = await _unitOfWork.BlogPosts.GetByIdAsync(request.Id, cancellationToken);
        if (blogPost is null)
            return Result.Failure("Blog post not found.");

        var result = blogPost.Archive();
        if (result.IsFailure)
            return result;

        _unitOfWork.BlogPosts.Update(blogPost);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
