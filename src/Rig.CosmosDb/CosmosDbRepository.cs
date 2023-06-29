using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Rig.Domain;

namespace Rig.CosmosDb;

public class CosmosDbRepository<T>(string databaseId, string containerId, CosmosClient client, Func<T, string> idSelector, Func<T, string> partitionKeySelector)
    : IRepository<T>
{
    public async ValueTask<IReadOnlyList<T>> Find(ISpecification<T> specification, CancellationToken cancellationToken)
    {
        var container = client.GetContainer(databaseId, containerId);

        var queryable = container.GetItemLinqQueryable<Document<T>>(
            linqSerializerOptions: new CosmosLinqSerializerOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            })
            .Where(doc => doc.DocumentType == typeof(T).Name)
            .Select(doc => doc.Data);

        var iterator = specification
            .Apply(queryable)
            .ToFeedIterator();

        var results = new List<T>();

        while (iterator.HasMoreResults)
        {
            var next = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(next);
        }

        return results;
    }

    public async ValueTask Save(IEnumerable<T> items, CancellationToken cancellationToken)
    {
        var partitionKey = items.Select(partitionKeySelector).Distinct().SingleOrDefault()
            ?? throw new InvalidOperationException("All items in a transaction must belong to the same partition");

        var container = client.GetContainer(databaseId, containerId);
        var batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));
        var typeName = typeof(T).Name;

        foreach (var item in items)
        {
            batch.UpsertItem(new Document<T>
            {
                Id = $"{typeName}-{idSelector(item)}",
                DocumentType = typeName,
                PartitionKey = partitionKey,
                Data = item
            });

            if (item is IDomainEventSource aggregateRoot)
            {
                var domainEvents = aggregateRoot.PublishEvents();
                foreach (var domainEvent in domainEvents)
                {
                    batch.CreateItem(new Document<DomainEvent>
                    {
                        Id = $"{nameof(DomainEvent)}-{Guid.NewGuid()}",
                        DocumentType = nameof(DomainEvent),
                        PartitionKey = partitionKey,
                        Data = domainEvent
                    });
                }
            }
        }

        var result = await batch.ExecuteAsync(cancellationToken);

        if (!result.IsSuccessStatusCode)
        {
            var errorMessage = result.ErrorMessage ??
                string.Join(
                    " ",
                    result.Where(r => !r.IsSuccessStatusCode).Select(r => r.StatusCode.ToString())
                );

            throw new Exception(errorMessage);
        }
    }
}
