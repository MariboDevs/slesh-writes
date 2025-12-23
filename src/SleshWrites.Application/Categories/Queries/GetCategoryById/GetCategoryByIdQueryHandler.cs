using SleshWrites.Application.Categories.DTOs;
using SleshWrites.Application.Categories.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Categories.Queries.GetCategoryById;

/// <summary>
/// Handler for getting a category by ID.
/// </summary>
public sealed class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result<CategoryDto>.Failure($"Category with ID '{request.Id}' not found.");
        }

        return Result<CategoryDto>.Success(category.ToDto());
    }
}
