using SleshWrites.Application.Authors.DTOs;
using SleshWrites.Application.Common.Interfaces;

namespace SleshWrites.Application.Authors.Queries.GetAuthorByAzureId;

/// <summary>
/// Query to get an author by Azure AD B2C ID.
/// </summary>
public sealed record GetAuthorByAzureIdQuery(string AzureAdB2CId) : IQuery<AuthorDto>;
