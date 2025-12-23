using MediatR;
using SleshWrites.API.Common;
using SleshWrites.API.Contracts.Categories;
using SleshWrites.Application.Categories.Commands.CreateCategory;
using SleshWrites.Application.Categories.Commands.DeleteCategory;
using SleshWrites.Application.Categories.Commands.UpdateCategory;
using SleshWrites.Application.Categories.Queries.GetAllCategories;
using SleshWrites.Application.Categories.Queries.GetCategoryById;

namespace SleshWrites.API.Endpoints;

/// <summary>
/// Minimal API endpoints for category operations.
/// </summary>
public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories")
            .WithOpenApi();

        group.MapGet("/", GetAll)
            .WithName("GetAllCategories")
            .WithSummary("Get all categories");

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetCategoryById")
            .WithSummary("Get a category by ID");

        group.MapPost("/", Create)
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        group.MapPut("/{id:guid}", Update)
            .WithName("UpdateCategory")
            .WithSummary("Update an existing category");

        group.MapDelete("/{id:guid}", Delete)
            .WithName("DeleteCategory")
            .WithSummary("Delete a category");
    }

    private static async Task<IResult> GetAll(
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllCategoriesQuery();
        var result = await mediator.Send(query, cancellationToken);
        return result.ToOkResult();
    }

    private static async Task<IResult> GetById(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result.ToNotFoundResult();
    }

    private static async Task<IResult> Create(
        IMediator mediator,
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description,
            request.DisplayOrder);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToCreatedResult($"/api/categories/{result.Value}");
    }

    private static async Task<IResult> Update(
        IMediator mediator,
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description,
            request.DisplayOrder);

        var result = await mediator.Send(command, cancellationToken);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> Delete(
        IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return result.ToDeleteResult();
    }
}
