using MediatR;
using SleshWrites.API.Common;
using SleshWrites.API.Contracts.Tags;
using SleshWrites.Application.Tags.Commands.CreateTag;
using SleshWrites.Application.Tags.Commands.DeleteTag;
using SleshWrites.Application.Tags.Commands.UpdateTag;
using SleshWrites.Application.Tags.Queries.GetAllTags;
using SleshWrites.Application.Tags.Queries.GetTagById;

namespace SleshWrites.API.Endpoints;

/// <summary>
/// Minimal API endpoints for tag operations.
/// </summary>
public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tags")
            .WithTags("Tags")
            .WithOpenApi();

        group.MapGet("/", GetAll)
            .WithName("GetAllTags")
            .WithSummary("Get all tags");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetTagById")
            .WithSummary("Get a tag by ID");

        group.MapPost("/", Create)
            .WithName("CreateTag")
            .WithSummary("Create a new tag");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateTag")
            .WithSummary("Update an existing tag");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteTag")
            .WithSummary("Delete a tag");
    }

    private static async Task<IResult> GetAll(
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllTagsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return result.ToOkResult();
    }

    private static async Task<IResult> GetById(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTagByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> Create(
        IMediator mediator,
        CreateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTagCommand(request.Name);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedResult($"/api/tags/{result.Value}");
    }

    private static async Task<IResult> Update(
        IMediator mediator,
        Guid id,
        UpdateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateTagCommand(id, request.Name);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Delete(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTagCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToDeleteResult();
    }
}
