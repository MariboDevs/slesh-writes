namespace SleshWrites.API.Contracts.Authors;

/// <summary>
/// Request model for creating a new author.
/// </summary>
public sealed record CreateAuthorRequest(
    string AzureAdB2CId,
    string DisplayName,
    string Email);
