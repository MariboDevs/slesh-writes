using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tags.Commands.CreateTag;

/// <summary>
/// Handler for creating a new tag.
/// </summary>
public sealed class CreateTagCommandHandler : ICommandHandler<CreateTagCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tagResult = Tag.Create(request.Name);
        if (tagResult.IsFailure)
        {
            return Result<Guid>.Failure(tagResult.Error);
        }

        var tag = tagResult.Value;

        // Check for duplicate slug (which is derived from name)
        var slugExists = await _unitOfWork.Tags.SlugExistsAsync(tag.Slug!.Value, null, cancellationToken);
        if (slugExists)
        {
            return Result<Guid>.Failure($"A tag with name '{request.Name}' already exists.");
        }

        _unitOfWork.Tags.Add(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tag.Id);
    }
}
