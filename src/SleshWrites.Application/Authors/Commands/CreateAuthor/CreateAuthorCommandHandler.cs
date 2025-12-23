using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Commands.CreateAuthor;

/// <summary>
/// Handler for creating a new author.
/// </summary>
public sealed class CreateAuthorCommandHandler : ICommandHandler<CreateAuthorCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuthorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate Azure AD B2C ID
        var exists = await _unitOfWork.Authors.AzureAdB2CIdExistsAsync(request.AzureAdB2CId, cancellationToken);
        if (exists)
        {
            return Result<Guid>.Failure($"An author with Azure AD B2C ID '{request.AzureAdB2CId}' already exists.");
        }

        var authorResult = Author.Create(request.AzureAdB2CId, request.DisplayName, request.Email);
        if (authorResult.IsFailure)
        {
            return Result<Guid>.Failure(authorResult.Error);
        }

        var author = authorResult.Value;
        _unitOfWork.Authors.Add(author);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(author.Id);
    }
}
