using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Authors.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Queries.GetAllAuthors;

/// <summary>
/// Handler for getting all authors.
/// </summary>
public sealed class GetAllAuthorsQueryHandler : IQueryHandler<GetAllAuthorsQuery, IReadOnlyList<AuthorDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllAuthorsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<AuthorDto>>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<AuthorDto>>.Success(authors.ToDtoList());
    }
}
