using Rig.Domain;

namespace Rig.Sample.Domain.Todos;

public class TodoById(Guid id) : ISpecification<Todo>
{
    public IQueryable<Todo> Apply(IQueryable<Todo> queryable) =>
        queryable.Where(todo => todo.Id == id);
}
