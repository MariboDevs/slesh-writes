using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Queries.GetAuthorById;

/// <summary>
/// Query to get an author by ID.
/// </summary>
public sealed record GetAuthorByIdQuery(Guid Id) : IQuery<AuthorDto>;
