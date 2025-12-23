using SleshWrites.API.Endpoints;
using SleshWrites.API.Middleware;
using SleshWrites.Application;
using SleshWrites.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add middleware
builder.Services.AddTransient<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map API endpoints
app.MapBlogPostEndpoints();
app.MapCategoryEndpoints();
app.MapTagEndpoints();
app.MapAuthorEndpoints();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
