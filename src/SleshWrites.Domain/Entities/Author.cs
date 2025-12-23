using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.Entities;

/// <summary>
/// Represents a blog author, linked to Azure AD B2C identity.
/// </summary>
public sealed class Author : Entity
{
    public string AzureAdB2CId { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }

    private readonly Dictionary<string, string> _socialLinks = [];
    public IReadOnlyDictionary<string, string> SocialLinks => _socialLinks.AsReadOnly();

    private Author() { }

    private Author(string azureAdB2CId, string displayName, string? email)
    {
        AzureAdB2CId = azureAdB2CId;
        DisplayName = displayName;
        Email = email;
    }

    public static Result<Author> Create(string azureAdB2CId, string displayName, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(azureAdB2CId))
            return Result.Failure<Author>("Azure AD B2C ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<Author>("Display name cannot be empty.");

        if (displayName.Length > 100)
            return Result.Failure<Author>("Display name cannot exceed 100 characters.");

        if (email is not null && email.Length > 256)
            return Result.Failure<Author>("Email cannot exceed 256 characters.");

        return Result.Success(new Author(azureAdB2CId, displayName, email));
    }

    public Result UpdateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure("Display name cannot be empty.");

        if (displayName.Length > 100)
            return Result.Failure("Display name cannot exceed 100 characters.");

        DisplayName = displayName;
        SetUpdatedAt();
        return Result.Success();
    }

    public void UpdateBio(string? bio)
    {
        Bio = bio?.Length > 1000 ? bio[..1000] : bio;
        SetUpdatedAt();
    }

    public Result UpdateAvatarUrl(string? avatarUrl)
    {
        if (avatarUrl is not null && !Uri.TryCreate(avatarUrl, UriKind.Absolute, out _))
            return Result.Failure("Avatar URL must be a valid absolute URL.");

        AvatarUrl = avatarUrl;
        SetUpdatedAt();
        return Result.Success();
    }

    public void AddSocialLink(string platform, string url)
    {
        if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(url))
            return;

        _socialLinks[platform.ToLowerInvariant()] = url;
        SetUpdatedAt();
    }

    public void RemoveSocialLink(string platform)
    {
        if (_socialLinks.Remove(platform.ToLowerInvariant()))
            SetUpdatedAt();
    }
}
