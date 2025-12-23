using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Application.Tags.DTOs;

namespace SleshWrites.Application.Tags.Queries.GetAllTags;

/// <summary>
/// Query to get all tags.
/// </summary>
public sealed record GetAllTagsQuery : IQuery<IReadOnlyList<TagDto>>;
