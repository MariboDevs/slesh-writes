using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Application.Tags.DTOs;
using SleshWrites.Application.Tags.Mappings;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tags.Queries.GetAllTags;

/// <summary>
/// Handler for getting all tags.
/// </summary>
public sealed class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, IReadOnlyList<TagDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTagsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<TagDto>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<TagDto>>.Success(tags.ToDtoList());
    }
}
