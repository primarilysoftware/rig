using Mediator;
using Rig.Domain;

namespace Rig.Sample.Domain.Todos;

public record CompleteTodoCommand : ICommand<Todo>
{
    public required Guid TodoId { get; init; }
}

public class CompleteTodoHandler(IRepository<Todo> todoRepo) : ICommandHandler<CompleteTodoCommand, Todo>
{
    public async ValueTask<Todo> Handle(CompleteTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepo.FindSingle(new TodoById(command.TodoId), cancellationToken)
            ?? throw new NotFoundException<Todo>(command.TodoId.ToString());

        todo.Complete(DateTimeOffset.UtcNow);

        await todoRepo.Save(todo, cancellationToken);

        return todo;
    }
}
