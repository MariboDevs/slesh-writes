using SleshWrites.Domain.Common;
using SleshWrites.Domain.ValueObjects;

namespace SleshWrites.Domain.Entities;

/// <summary>
/// Represents a blog post category.
/// </summary>
public sealed class Category : Entity
{
    public string Name { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }

    private Category() { }

    private Category(string name, Slug slug, string? description, int displayOrder)
    {
        Name = name;
        Slug = slug;
        Description = description;
        DisplayOrder = displayOrder;
    }

    public static Result<Category> Create(string name, string? description = null, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Category>("Category name cannot be empty.");

        if (name.Length > 100)
            return Result.Failure<Category>("Category name cannot exceed 100 characters.");

        if (description?.Length > 500)
            return Result.Failure<Category>("Category description cannot exceed 500 characters.");

        var slugResult = Slug.Create(name);
        if (slugResult.IsFailure)
            return Result.Failure<Category>($"Failed to create slug: {slugResult.Error}");

        return Result.Success(new Category(name, slugResult.Value, description, displayOrder));
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Category name cannot be empty.");

        if (name.Length > 100)
            return Result.Failure("Category name cannot exceed 100 characters.");

        var slugResult = Slug.Create(name);
        if (slugResult.IsFailure)
            return Result.Failure($"Failed to create slug: {slugResult.Error}");

        Name = name;
        Slug = slugResult.Value;
        SetUpdatedAt();
        return Result.Success();
    }

    public Result UpdateDescription(string? description)
    {
        if (description is not null && description.Length > 500)
            return Result.Failure("Category description cannot exceed 500 characters.");

        Description = description;
        SetUpdatedAt();
        return Result.Success();
    }

    public Result UpdateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
            return Result.Failure("Display order cannot be negative.");

        DisplayOrder = displayOrder;
        SetUpdatedAt();
        return Result.Success();
    }
}
