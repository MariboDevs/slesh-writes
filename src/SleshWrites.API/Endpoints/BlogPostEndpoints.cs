using MediatR;
using SleshWrites.API.Common;
using SleshWrites.API.Contracts.BlogPosts;
using SleshWrites.Application.BlogPosts.Commands.ArchiveBlogPost;
using SleshWrites.Application.BlogPosts.Commands.CreateBlogPost;
using SleshWrites.Application.BlogPosts.Commands.DeleteBlogPost;
using SleshWrites.Application.BlogPosts.Commands.PublishBlogPost;
using SleshWrites.Application.BlogPosts.Commands.UpdateBlogPost;
using SleshWrites.Application.BlogPosts.Queries.GetBlogPostById;
using SleshWrites.Application.BlogPosts.Queries.GetBlogPostBySlug;
using SleshWrites.Application.BlogPosts.Queries.GetBlogPostsPaged;
using SleshWrites.Application.BlogPosts.Queries.GetRecentBlogPosts;
using SleshWrites.Domain.Enums;

namespace SleshWrites.API.Endpoints;

/// <summary>
/// Minimal API endpoints for blog post operations.
/// </summary>
public static class BlogPostEndpoints
{
    public static void MapBlogPostEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/blogposts")
            .WithTags("BlogPosts")
            .WithOpenApi();

        group.MapGet("/", GetPaged)
            .WithName("GetBlogPostsPaged")
            .WithSummary("Get paginated blog posts");

        group.MapGet("/recent", GetRecent)
            .WithName("GetRecentBlogPosts")
            .WithSummary("Get recent published blog posts");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetBlogPostById")
            .WithSummary("Get a blog post by ID");

        group.MapGet("/slug/{slug}", GetBySlug)
            .WithName("GetBlogPostBySlug")
            .WithSummary("Get a blog post by slug");

        group.MapPost("/", Create)
            .WithName("CreateBlogPost")
            .WithSummary("Create a new blog post");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateBlogPost")
            .WithSummary("Update an existing blog post");

        group.MapPost("/{id:guid}/publish", Publish)
            .WithName("PublishBlogPost")
            .WithSummary("Publish a blog post");

        group.MapPost("/{id:guid}/archive", Archive)
            .WithName("ArchiveBlogPost")
            .WithSummary("Archive a blog post");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteBlogPost")
            .WithSummary("Delete a blog post");
    }

    private static async Task<IResult> GetPaged(
        IMediator mediator,
        int pageNumber = 1,
        int pageSize = 10,
        PostStatus? status = null,
        Guid? categoryId = null,
        Guid? authorId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBlogPostsPagedQuery(pageNumber, pageSize, status, categoryId, authorId);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToOkResult();
    }

    private static async Task<IResult> GetRecent(
        IMediator mediator,
        int count = 5,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRecentBlogPostsQuery(count);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToOkResult();
    }

    private static async Task<IResult> GetById(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBlogPostByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> GetBySlug(
        IMediator mediator,
        string slug,
        CancellationToken cancellationToken = default)
    {
        var query = new GetBlogPostBySlugQuery(slug);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> Create(
        IMediator mediator,
        CreateBlogPostRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateBlogPostCommand(
            request.Title,
            request.Content,
            request.AuthorId,
            request.CategoryId,
            request.Excerpt,
            request.FeaturedImage,
            request.TagIds);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedResult($"/api/blogposts/{result.Value}");
    }

    private static async Task<IResult> Update(
        IMediator mediator,
        Guid id,
        UpdateBlogPostRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateBlogPostCommand(
            id,
            request.Title,
            request.Content,
            request.Excerpt,
            request.FeaturedImage,
            request.CategoryId,
            request.TagIds);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Publish(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new PublishBlogPostCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Archive(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new ArchiveBlogPostCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Delete(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBlogPostCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToDeleteResult();
    }
}
