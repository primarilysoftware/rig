using Rig.Domain;

namespace Rig.Sample.Domain.Todos;

public class Todo : AggregateRoot
{
    public required Guid Id { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public bool Completed { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Completed)
        {
            return;
        }

        Completed = true;
        CompletedAt = completedAt;

        RecordEvent(new TodoCompleted
        {
            TodoId = Id,
            CompletedAt = completedAt
        });
    }
}

public record TodoCompleted : DomainEvent
{
    public required Guid TodoId { get; init; }
    public required DateTimeOffset CompletedAt { get; init; }
}