using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tags.Commands.DeleteTag;

/// <summary>
/// Handler for deleting a tag.
/// </summary>
public sealed class DeleteTagCommandHandler : ICommandHandler<DeleteTagCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTagCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken);
        if (tag is null)
        {
            return Result.Failure($"Tag with ID '{request.Id}' not found.");
        }

        _unitOfWork.Tags.Remove(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
