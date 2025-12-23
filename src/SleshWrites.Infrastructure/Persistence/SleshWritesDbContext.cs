using Microsoft.EntityFrameworkCore;
using SleshWrites.Domain.Entities;

namespace SleshWrites.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for SleshWrites blog platform.
/// </summary>
public sealed class SleshWritesDbContext : DbContext
{
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();

    public SleshWritesDbContext(DbContextOptions<SleshWritesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SleshWritesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
