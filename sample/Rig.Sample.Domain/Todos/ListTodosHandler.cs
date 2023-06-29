using Mediator;
using Rig.Domain;

namespace Rig.Sample.Domain.Todos;

public record ListTodosQuery : IQuery<IReadOnlyList<Todo>>
{

}

public class ListTodosHandler(IRepository<Todo> todoRepo) : IQueryHandler<ListTodosQuery, IReadOnlyList<Todo>>
{
    public async ValueTask<IReadOnlyList<Todo>> Handle(ListTodosQuery query, CancellationToken cancellationToken)
    {
        var todos = await todoRepo.FindAll(cancellationToken);
        return todos;
    }
}
