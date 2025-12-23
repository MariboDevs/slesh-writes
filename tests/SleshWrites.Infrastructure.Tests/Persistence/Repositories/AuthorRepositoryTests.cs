using FluentAssertions;
using SleshWrites.Domain.Entities;
using SleshWrites.Infrastructure.Persistence;
using SleshWrites.Infrastructure.Persistence.Repositories;

namespace SleshWrites.Infrastructure.Tests.Persistence.Repositories;

public class AuthorRepositoryTests : IDisposable
{
    private readonly SleshWritesDbContext _context;
    private readonly AuthorRepository _repository;

    public AuthorRepositoryTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new AuthorRepository(_context);
    }

    [Fact]
    public async Task Add_AndGetById_ReturnsAuthor()
    {
        // Arrange
        var author = Author.Create("azure-id-123", "John Doe", "john@example.com").Value;

        // Act
        _repository.Add(author);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("John Doe");
        result.AzureAdB2CId.Should().Be("azure-id-123");
    }

    [Fact]
    public async Task GetByAzureAdB2CIdAsync_WithExistingId_ReturnsAuthor()
    {
        // Arrange
        var author = Author.Create("unique-azure-id", "Jane Doe").Value;
        _repository.Add(author);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAzureAdB2CIdAsync("unique-azure-id");

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task GetByAzureAdB2CIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByAzureAdB2CIdAsync("non-existent-azure-id");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AzureAdB2CIdExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var author = Author.Create("existing-azure-id", "Test Author").Value;
        _repository.Add(author);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.AzureAdB2CIdExistsAsync("existing-azure-id");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AzureAdB2CIdExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Act
        var result = await _repository.AzureAdB2CIdExistsAsync("non-existent-azure-id");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Update_ModifiesAuthor()
    {
        // Arrange
        var author = Author.Create("azure-id", "Original Name").Value;
        _repository.Add(author);
        await _context.SaveChangesAsync();

        // Act
        author.UpdateDisplayName("Updated Name");
        author.UpdateBio("This is a new bio.");
        _repository.Update(author);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result!.DisplayName.Should().Be("Updated Name");
        result.Bio.Should().Be("This is a new bio.");
    }

    [Fact]
    public async Task Author_WithSocialLinks_PersistsCorrectly()
    {
        // Arrange
        var author = Author.Create("azure-id", "Social Author").Value;
        author.AddSocialLink("twitter", "https://twitter.com/test");
        author.AddSocialLink("github", "https://github.com/test");

        _repository.Add(author);
        await _context.SaveChangesAsync();

        // Detach and reload
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByIdAsync(author.Id);

        // Assert
        result.Should().NotBeNull();
        result!.SocialLinks.Should().HaveCount(2);
        result.SocialLinks.Should().ContainKey("twitter");
        result.SocialLinks.Should().ContainKey("github");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
