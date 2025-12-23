using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Application.Tags.DTOs;
using SleshWrites.Application.Tags.Mappings;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tags.Queries.GetTagById;

/// <summary>
/// Handler for getting a tag by ID.
/// </summary>
public sealed class GetTagByIdQueryHandler : IQueryHandler<GetTagByIdQuery, TagDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTagByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TagDto>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return Result<TagDto>.Failure($"Tag with ID '{request.Id}' not found.");
        }

        return Result<TagDto>.Success(tag.ToDto());
    }
}
