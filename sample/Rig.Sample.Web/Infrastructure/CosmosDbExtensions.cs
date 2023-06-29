using Microsoft.Azure.Cosmos;
using Rig.CosmosDb;
using Rig.Domain;
using Rig.Sample.Domain.Todos;

namespace Rig.Sample.Web.Infrastructure;

public static class CosmosDbExtensions
{
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        builder.RigCosmosDb(builder.Configuration.GetValue<string>("CosmosDbConnectionString")!);

        builder.Services.AddTransient<IRepository<Todo>>(services =>
        {
            return new CosmosDbRepository<Todo>(
                "sample",
                "todos",
                services.GetRequiredService<CosmosClient>(),
                todo => todo.Id.ToString(),
                todo => todo.Id.ToString());
        });
    }

    public static async Task InitializeDatabase(this WebApplication app)
    {
        var client = app.Services.GetRequiredService<CosmosClient>();
        var database = await client.CreateDatabaseIfNotExistsAsync("sample");
        await database.Database.CreateContainerIfNotExistsAsync("todos", "/partitionKey");
    }
}
