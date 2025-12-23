using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Commands.DeleteAuthor;

/// <summary>
/// Handler for deleting an author.
/// </summary>
public sealed class DeleteAuthorCommandHandler : ICommandHandler<DeleteAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAuthorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
        {
            return Result.Failure($"Author with ID '{request.Id}' not found.");
        }

        // Check if author has any blog posts
        var blogPosts = await _unitOfWork.BlogPosts.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1,
            status: null,
            categoryId: null,
            authorId: request.Id,
            cancellationToken);

        if (blogPosts.TotalCount > 0)
        {
            return Result.Failure("Cannot delete author that has associated blog posts.");
        }

        _unitOfWork.Authors.Remove(author);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
