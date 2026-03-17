namespace ProjectLucien.Domain.Events;

/// <summary>
/// Base interface for all domain events.
/// Domain events represent significant occurrences within the domain that other parts of the system may be interested in.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
}
