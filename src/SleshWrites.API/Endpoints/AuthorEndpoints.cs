using MediatR;
using SleshWrites.API.Common;
using SleshWrites.API.Contracts.Authors;
using SleshWrites.Application.Authors.Commands.CreateAuthor;
using SleshWrites.Application.Authors.Commands.DeleteAuthor;
using SleshWrites.Application.Authors.Commands.UpdateAuthor;
using SleshWrites.Application.Authors.Queries.GetAllAuthors;
using SleshWrites.Application.Authors.Queries.GetAuthorByAzureId;
using SleshWrites.Application.Authors.Queries.GetAuthorById;

namespace SleshWrites.API.Endpoints;

/// <summary>
/// Minimal API endpoints for author operations.
/// </summary>
public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authors")
            .WithTags("Authors")
            .WithOpenApi();

        group.MapGet("/", GetAll)
            .WithName("GetAllAuthors")
            .WithSummary("Get all authors");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetAuthorById")
            .WithSummary("Get an author by ID");

        group.MapGet("/azure/{azureId}", GetByAzureId)
            .WithName("GetAuthorByAzureId")
            .WithSummary("Get an author by Azure AD B2C ID");

        group.MapPost("/", Create)
            .WithName("CreateAuthor")
            .WithSummary("Create a new author");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateAuthor")
            .WithSummary("Update an existing author");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteAuthor")
            .WithSummary("Delete an author");
    }

    private static async Task<IResult> GetAll(
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllAuthorsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return result.ToOkResult();
    }

    private static async Task<IResult> GetById(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuthorByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> GetByAzureId(
        IMediator mediator,
        string azureId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAuthorByAzureIdQuery(azureId);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> Create(
        IMediator mediator,
        CreateAuthorRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateAuthorCommand(
            request.AzureAdB2CId,
            request.DisplayName,
            request.Email);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedResult($"/api/authors/{result.Value}");
    }

    private static async Task<IResult> Update(
        IMediator mediator,
        Guid id,
        UpdateAuthorRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateAuthorCommand(
            id,
            request.DisplayName,
            request.Bio,
            request.AvatarUrl);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Delete(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteAuthorCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToDeleteResult();
    }
}
