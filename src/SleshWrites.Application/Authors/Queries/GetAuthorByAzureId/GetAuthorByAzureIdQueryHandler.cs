using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Authors.Mappings;
using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Queries.GetAuthorByAzureId;

/// <summary>
/// Handler for getting an author by Azure AD B2C ID.
/// </summary>
public sealed class GetAuthorByAzureIdQueryHandler : IQueryHandler<GetAuthorByAzureIdQuery, AuthorDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuthorByAzureIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthorDto>> Handle(GetAuthorByAzureIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.Authors.GetByAzureAdB2CIdAsync(request.AzureAdB2CId, cancellationToken);
        if (author is null)
        {
            return Result<AuthorDto>.Failure($"Author with Azure AD B2C ID '{request.AzureAdB2CId}' not found.");
        }

        return Result<AuthorDto>.Success(author.ToDto());
    }
}
