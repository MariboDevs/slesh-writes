using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Tags.Commands.DeleteTag;

/// <summary>
/// Command to delete a tag.
/// </summary>
public sealed record DeleteTagCommand(Guid Id) : ICommand;
