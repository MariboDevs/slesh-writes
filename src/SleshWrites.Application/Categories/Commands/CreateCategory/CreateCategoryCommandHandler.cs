using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Application.Categories.Commands.CreateCategory;

/// <summary>
/// Handler for creating a new category.
/// </summary>
public sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryResult = Category.Create(request.Name, request.Description, request.DisplayOrder);
        if (categoryResult.IsFailure)
        {
            return Result<Guid>.Failure(categoryResult.Error);
        }

        var category = categoryResult.Value;

        // Check for duplicate slug (which is derived from name)
        var slugExists = await _unitOfWork.Categories.SlugExistsAsync(category.Slug!.Value, null, cancellationToken);
        if (slugExists)
        {
            return Result<Guid>.Failure($"A category with name '{request.Name}' already exists.");
        }

        _unitOfWork.Categories.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}
