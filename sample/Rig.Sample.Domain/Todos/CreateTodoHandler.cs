using Mediator;
using Rig.Domain;

namespace Rig.Sample.Domain.Todos;

public record CreateTodoCommand : ICommand<Todo>
{
    public required TodoRequest Todo { get; init; }
}

public record TodoRequest
{
    public required string Description { get; init; }
}

public class CreateTodoHandler(IRepository<Todo> todoRepo) : ICommandHandler<CreateTodoCommand, Todo>
{
    public async ValueTask<Todo> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Description = command.Todo.Description,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await todoRepo.Save(todo, cancellationToken);

        return todo;
    }
}
