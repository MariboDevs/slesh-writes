using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Authors.Commands.UpdateAuthor;

/// <summary>
/// Handler for updating an existing author.
/// </summary>
public sealed class UpdateAuthorCommandHandler : ICommandHandler<UpdateAuthorCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAuthorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
        {
            return Result.Failure($"Author with ID '{request.Id}' not found.");
        }

        var displayNameResult = author.UpdateDisplayName(request.DisplayName);
        if (displayNameResult.IsFailure)
        {
            return displayNameResult;
        }

        var bioResult = author.UpdateBio(request.Bio);
        if (bioResult.IsFailure)
        {
            return bioResult;
        }

        var avatarResult = author.UpdateAvatarUrl(request.AvatarUrl);
        if (avatarResult.IsFailure)
        {
            return avatarResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
