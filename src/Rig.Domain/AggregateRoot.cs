namespace Rig.Domain;

public abstract class AggregateRoot : IDomainEventSource
{
    private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();

    public IReadOnlyList<DomainEvent> PublishEvents()
    {
        var events = _domainEvents.ToArray();
        _domainEvents.Clear();
        return events;
    }

    protected void RecordEvent<T>(T @event) where T : DomainEvent
    {
        _domainEvents.Add(@event);
    }
}
