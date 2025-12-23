using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Authors.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Queries.GetAuthorById;

/// <summary>
/// Handler for getting an author by ID.
/// </summary>
public sealed class GetAuthorByIdQueryHandler : IQueryHandler<GetAuthorByIdQuery, AuthorDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
        {
            return Result<AuthorDto>.Failure($"Author with ID '{request.Id}' not found.");
        }

        return Result<AuthorDto>.Success(author.ToDto());
    }
}
