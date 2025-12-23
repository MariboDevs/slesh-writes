using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Application.Categories.Commands.UpdateCategory;

/// <summary>
/// Handler for updating an existing category.
/// </summary>
public sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure($"Category with ID '{request.Id}' not found.");
        }

        // Check for duplicate slug (exclude current category)
        var newSlug = Slug.Create(request.Name);
        if (newSlug.IsSuccess)
        {
            var slugExists = await _unitOfWork.Categories.SlugExistsAsync(newSlug.Value.Value, request.Id, cancellationToken);
            if (slugExists)
            {
                return Result.Failure($"A category with name '{request.Name}' already exists.");
            }
        }

        var nameResult = category.UpdateName(request.Name);
        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        var descriptionResult = category.UpdateDescription(request.Description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult;
        }

        if (request.DisplayOrder.HasValue)
        {
            var displayOrderResult = category.UpdateDisplayOrder(request.DisplayOrder.Value);
            if (displayOrderResult.IsFailure)
            {
                return displayOrderResult;
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
