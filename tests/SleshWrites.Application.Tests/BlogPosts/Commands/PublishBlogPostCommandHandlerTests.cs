using FluentAssertions;
using NSubstitute;
using SleshWrites.Application.BlogPosts.Commands.PublishBlogPost;
using SleshWrites.Domain.Entities;
using SleshWrites.Domain.Enums;
using SleshWrites.Domain.Repositories;

namespace SleshWrites.Application.Tests.BlogPosts.Commands;

public class PublishBlogPostCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly PublishBlogPostCommandHandler _handler;

    public PublishBlogPostCommandHandlerTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _blogPostRepository = Substitute.For<IBlogPostRepository>();

        _unitOfWork.BlogPosts.Returns(_blogPostRepository);

        _handler = new PublishBlogPostCommandHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_WithDraftPost_PublishesSuccessfully()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Test Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        var postId = blogPost.Id;

        _blogPostRepository.GetByIdAsync(postId, Arg.Any<CancellationToken>())
            .Returns(blogPost);

        var command = new PublishBlogPostCommand(postId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        blogPost.Status.Should().Be(PostStatus.Published);
        blogPost.PublishedAt.Should().NotBeNull();
        _blogPostRepository.Received(1).Update(blogPost);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        var postId = Guid.NewGuid();

        _blogPostRepository.GetByIdAsync(postId, Arg.Any<CancellationToken>())
            .Returns((BlogPost?)null);

        var command = new PublishBlogPostCommand(postId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithAlreadyPublishedPost_ReturnsFailure()
    {
        // Arrange
        var blogPost = BlogPost.Create(
            "Test Post",
            "This is the content of the test blog post. It needs to be at least 50 characters.",
            Guid.NewGuid(),
            Guid.NewGuid()).Value;

        blogPost.Publish(); // Already published

        var postId = blogPost.Id;

        _blogPostRepository.GetByIdAsync(postId, Arg.Any<CancellationToken>())
            .Returns(blogPost);

        var command = new PublishBlogPostCommand(postId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already published");
    }
}
