using FluentAssertions;
using NSubstitute;
using SleshWrites.Application.BlogPosts.Queries.GetBlogPostById;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tests.BlogPosts.Queries;

public class GetBlogPostByIdQueryHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly GetBlogPostByIdQueryHandler _handler;

    public GetBlogPostByIdQueryHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _blogPostRepository = Substitute.For<IBlogPostRepository>();
        _authorRepository = Substitute.For<IAuthorRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();

        _unitOfWork.BlogPosts.Returns(_blogPostRepository);
        _unitOfWork.Authors.Returns(_authorRepository);
        _unitOfWork.Categories.Returns(_categoryRepository);

        _handler = new GetBlogPostByIdQueryHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithExistingPost_ReturnsBlogPostDto()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var blogPost = BlogPost.Create(
            "Test Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            authorId,
            categoryId).Value;

        var author = Author.Create("azure-id", "Test Author").Value;
        var category = Category.Create("Technology").Value;

        _blogPostRepository.GetByIdWithTagsAsync(blogPost.Id, Arg.Any<CancellationToken>())
            .Returns(blogPost);
        _authorRepository.GetByIdAsync(authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        var query = new GetBlogPostByIdQuery(blogPost.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(blogPost.Id);
        result.Value.Title.Should().Be("Test Post");
        result.Value.AuthorName.Should().Be("Test Author");
        result.Value.CategoryName.Should().Be("Technology");
    }

    [Fact]
    public async Task Handle_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        var postId = Guid.NewGuid();

        _blogPostRepository.GetByIdWithTagsAsync(postId, Arg.Any<CancellationToken>())
            .Returns((BlogPost?)null);

        var query = new GetBlogPostByIdQuery(postId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }
}
