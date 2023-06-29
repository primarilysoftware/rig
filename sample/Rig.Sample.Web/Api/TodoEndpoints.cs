using Rig.Api;
using Rig.Sample.Domain.Todos;

namespace Rig.Sample.Web.Api;

public static class TodoEndpoints
{
    public static RouteGroupBuilder MapTodoEndpoints(this RouteGroupBuilder api)
    {
        var todos = api.MapGroup("todos")
            .WithTags("Todos");

        todos.Mediate<CreateTodoCommand>(HttpMethod.Post);
        todos.Mediate<ListTodosQuery>(HttpMethod.Get);
        todos.Mediate<CompleteTodoCommand>(HttpMethod.Post, "complete");

        return api;
    }
}
