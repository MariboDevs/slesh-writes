using FluentAssertions;
using NSubstitute;
using SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tests.BlogPosts.Commands;

public class CreateBlogPostCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITagRepository _tagRepository;
    private readonly CreateBlogPostCommandHandler _handler;

    private readonly Guid _authorId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public CreateBlogPostCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _blogPostRepository = Substitute.For<IBlogPostRepository>();
        _authorRepository = Substitute.For<IAuthorRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _tagRepository = Substitute.For<ITagRepository>();

        _unitOfWork.BlogPosts.Returns(_blogPostRepository);
        _unitOfWork.Authors.Returns(_authorRepository);
        _unitOfWork.Categories.Returns(_categoryRepository);
        _unitOfWork.Tags.Returns(_tagRepository);

        _handler = new CreateBlogPostCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessWithBlogPostId()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;
        var category = Category.Create("Technology").Value;

        _authorRepository.GetByIdAsync(_authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _categoryRepository.GetByIdAsync(_categoryId, Arg.Any<CancellationToken>())
            .Returns(category);
        _tagRepository.GetByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var command = new CreateBlogPostCommand(
            Title: "Test Blog Post",
            Content: "This is the content of the test blog post. It needs to be at least 50 characters.",
            AuthorId: _authorId,
            CategoryId: _categoryId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        _blogPostRepository.Received(1).Add(Arg.Any<BlogPost>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentAuthor_ReturnsFailure()
    {
        // Arrange
        _authorRepository.GetByIdAsync(_authorId, Arg.Any<CancellationToken>())
            .Returns((Author?)null);

        var command = new CreateBlogPostCommand(
            Title: "Test Blog Post",
            Content: "This is the content of the test blog post. It needs to be at least 50 characters.",
            AuthorId: _authorId,
            CategoryId: _categoryId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Author not found");
    }

    [Fact]
    public async Task Handle_WithNonExistentCategory_ReturnsFailure()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;

        _authorRepository.GetByIdAsync(_authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _categoryRepository.GetByIdAsync(_categoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new CreateBlogPostCommand(
            Title: "Test Blog Post",
            Content: "This is the content of the test blog post. It needs to be at least 50 characters.",
            AuthorId: _authorId,
            CategoryId: _categoryId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Category not found");
    }

    [Fact]
    public async Task Handle_WithTags_AddsTags()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;
        var category = Category.Create("Technology").Value;
        var tag1 = Tag.Create("CSharp").Value;
        var tag2 = Tag.Create("DotNet").Value;
        var tagIds = new List<Guid> { tag1.Id, tag2.Id };

        _authorRepository.GetByIdAsync(_authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _categoryRepository.GetByIdAsync(_categoryId, Arg.Any<CancellationToken>())
            .Returns(category);
        _tagRepository.GetByIdsAsync(tagIds, Arg.Any<CancellationToken>())
            .Returns([tag1, tag2]);

        var command = new CreateBlogPostCommand(
            Title: "Test Blog Post",
            Content: "This is the content of the test blog post. It needs to be at least 50 characters.",
            AuthorId: _authorId,
            CategoryId: _categoryId,
            TagIds: tagIds);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _blogPostRepository.Received(1).Add(Arg.Is<BlogPost>(p => p.Tags.Count == 2));
    }

    [Fact]
    public async Task Handle_WithFeaturedImage_SetsFeaturedImage()
    {
        // Arrange
        var author = Author.Create("azure-id", "Test Author").Value;
        var category = Category.Create("Technology").Value;
        var imageUrl = "https://example.com/image.jpg";

        _authorRepository.GetByIdAsync(_authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _categoryRepository.GetByIdAsync(_categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        var command = new CreateBlogPostCommand(
            Title: "Test Blog Post",
            Content: "This is the content of the test blog post. It needs to be at least 50 characters.",
            AuthorId: _authorId,
            CategoryId: _categoryId,
            FeaturedImage: imageUrl);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _blogPostRepository.Received(1).Add(Arg.Is<BlogPost>(p => p.FeaturedImage == imageUrl));
    }
}
