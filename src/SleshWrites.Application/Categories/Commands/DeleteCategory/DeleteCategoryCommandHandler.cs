using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Categories.Commands.DeleteCategory;

/// <summary>
/// Handler for deleting a category.
/// </summary>
public sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure($"Category with ID '{request.Id}' not found.");
        }

        // Check if category has any blog posts
        var hasBlogPosts = await _unitOfWork.Categories.HasBlogPostsAsync(request.Id, cancellationToken);
        if (hasBlogPosts)
        {
            return Result.Failure("Cannot delete category that has associated blog posts.");
        }

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
