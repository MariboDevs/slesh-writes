using SleshWrites.Domain.Common;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Domain.Entities;

/// <summary>
/// Represents a tag that can be associated with blog posts.
/// </summary>
public sealed class Tag : Entity
{
    public string Name { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;

    private Tag() { }

    private Tag(string name, Slug slug)
    {
        Name = name;
        Slug = slug;
    }

    public static Result<Tag> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Tag>("Tag name cannot be empty.");

        if (name.Length > 50)
            return Result.Failure<Tag>("Tag name cannot exceed 50 characters.");

        var slugResult = Slug.Create(name);
        if (slugResult.IsFailure)
            return Result.Failure<Tag>($"Failed to create slug: {slugResult.Error}");

        return Result.Success(new Tag(name, slugResult.Value));
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Tag name cannot be empty.");

        if (name.Length > 50)
            return Result.Failure("Tag name cannot exceed 50 characters.");

        var slugResult = Slug.Create(name);
        if (slugResult.IsFailure)
            return Result.Failure($"Failed to create slug: {slugResult.Error}");

        Name = name;
        Slug = slugResult.Value;
        SetUpdatedAt();
        return Result.Success();
    }
}
