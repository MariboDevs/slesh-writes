using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Command to update an existing tag.
/// </summary>
public sealed record UpdateTagCommand(Guid Id, string Name) : ICommand;
