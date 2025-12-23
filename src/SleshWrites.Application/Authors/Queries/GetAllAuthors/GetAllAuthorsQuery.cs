using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Queries.GetAllAuthors;

/// <summary>
/// Query to get all authors.
/// </summary>
public sealed record GetAllAuthorsQuery : IQuery<IReadOnlyList<AuthorDto>>;
