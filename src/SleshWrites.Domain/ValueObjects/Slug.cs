using System.Text.RegularExpressions;
using SleshWrites.Domain.Common;

namespace SleshWrites.Domain.ValueObjects;

/// <summary>
/// Represents a URL-friendly slug for blog posts and categories.
/// Slugs are lowercase, contain only alphanumeric characters and hyphens.
/// </summary>
public sealed partial class Slug : ValueObject
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Slug from the given input string.
    /// </summary>
    /// <param name="input">The input string to convert to a slug.</param>
    /// <returns>A Result containing the Slug if valid, or an error message if invalid.</returns>
    public static Result<Slug> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Failure<Slug>("Slug cannot be empty.");

        var slug = GenerateSlug(input);

        if (string.IsNullOrWhiteSpace(slug))
            return Result.Failure<Slug>("Generated slug is empty. Input must contain at least one alphanumeric character.");

        if (slug.Length > 200)
            return Result.Failure<Slug>("Slug cannot exceed 200 characters.");

        if (!SlugRegex().IsMatch(slug))
            return Result.Failure<Slug>("Slug must contain only lowercase letters, numbers, and hyphens.");

        return Result.Success(new Slug(slug));
    }

    /// <summary>
    /// Creates a Slug from a pre-validated slug string (e.g., from database).
    /// </summary>
    public static Slug FromExisting(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be empty.", nameof(slug));

        return new Slug(slug);
    }

    private static string GenerateSlug(string input)
    {
        // Convert to lowercase
        var slug = input.ToLowerInvariant();

        // Replace spaces and underscores with hyphens
        slug = slug.Replace(' ', '-').Replace('_', '-');

        // Remove special characters, keeping only alphanumeric and hyphens
        slug = NonAlphanumericRegex().Replace(slug, "");

        // Replace multiple consecutive hyphens with a single hyphen
        slug = MultipleHyphensRegex().Replace(slug, "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        return slug;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.Value;

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugRegex();

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex MultipleHyphensRegex();
}
