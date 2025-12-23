namespace SleshWrites.API.Contracts.Authors;

/// <summary>
/// Request model for updating an existing author.
/// </summary>
public sealed record UpdateAuthorRequest(
    string DisplayName,
    string? Bio,
    string? AvatarUrl);
