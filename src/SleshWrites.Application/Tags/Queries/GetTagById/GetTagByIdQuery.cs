using SleshWrites.Application.Common.Interfaces;
using SleshWrites.Application.Tags.DTOs;

namespace SleshWrites.Application.Tags.Queries.GetTagById;

/// <summary>
/// Query to get a tag by ID.
/// </summary>
public sealed record GetTagByIdQuery(Guid Id) : IQuery<TagDto>;
