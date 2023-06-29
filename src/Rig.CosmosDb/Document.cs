namespace Rig.CosmosDb;

public record Document<T>
{
    public required string DocumentType { get; init; }
    public required string Id { get; init; }
    public required string PartitionKey { get; init; }
    public required T Data { get; init; }
}
