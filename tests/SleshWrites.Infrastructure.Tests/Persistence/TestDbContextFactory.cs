using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SleshWrites.Infrastructure.Persistence;

namespace SleshWrites.Infrastructure.Tests.Persistence;

/// <summary>
/// Factory for creating in-memory DbContext instances for testing.
/// </summary>
public static class TestDbContextFactory
{
    public static SleshWritesDbContext Create()
    {
        var options = new DbContextOptionsBuilder<SleshWritesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new SleshWritesDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
