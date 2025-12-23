namespace SleshWrites.Domain.Common;

/// <summary>
/// Marker interface for aggregate roots.
/// An aggregate root is the entry point to an aggregate - a cluster of domain objects that can be treated as a single unit.
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
