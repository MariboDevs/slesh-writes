using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Handler for updating an existing tag.
/// </summary>
public sealed class UpdateTagCommandHandler : ICommandHandler<UpdateTagCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return Result.Failure($"Tag with ID '{request.Id}' not found.");
        }

        // Check for duplicate slug (exclude current tag)
        var newSlug = Slug.Create(request.Name);
        if (newSlug.IsSuccess)
        {
            var slugExists = await _unitOfWork.Tags.SlugExistsAsync(newSlug.Value.Value, request.Id, cancellationToken);
            if (slugExists)
            {
                return Result.Failure($"A tag with name '{request.Name}' already exists.");
            }
        }

        var updateResult = tag.UpdateName(request.Name);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
