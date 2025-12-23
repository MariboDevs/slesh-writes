using MediatR;
using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Common;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for SleshWrites blog platform.
/// Includes domain event dispatching on SaveChanges.
/// </summary>
public sealed class SleshWritesDbContext : DbContext
{
    private readonly IMediator? _mediator;

    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    public SleshWritesDbContext(DbContextOptions<SleshWritesDbContext> options)
        : base(options)
    {
    }

    public SleshWritesDbContext(
        DbContextOptions<SleshWritesDbContext> options,
        IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SleshWritesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events before saving
        var domainEvents = GetDomainEvents();

        // Save changes
        var result = await base.SaveChangesAsync(cancellationToken);

        // Dispatch domain events after successful save
        await DispatchDomainEventsAsync(domainEvents, cancellationToken);

        return result;
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        var entities = ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear domain events from entities
        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        return domainEvents;
    }

    private async Task DispatchDomainEventsAsync(
        List<IDomainEvent> domainEvents,
        CancellationToken cancellationToken)
    {
        if (_mediator is null || domainEvents.Count == 0)
        {
            return;
        }

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
