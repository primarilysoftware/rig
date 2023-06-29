namespace Rig.Domain;

public interface IDomainEventSource
{
    IReadOnlyList<DomainEvent> PublishEvents();
}
