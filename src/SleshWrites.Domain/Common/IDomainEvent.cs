namespace SleshWrites.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain that other parts of the system might be interested in.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
