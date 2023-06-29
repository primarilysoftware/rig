namespace Rig.Domain;

public static class IRepositoryExtensions
{
    public static ValueTask Save<T>(this IRepository<T> repo, T item, CancellationToken cancellationToken)
    {
        return repo.Save(new[] { item }, cancellationToken);
    }

    public static ValueTask<IReadOnlyList<T>> FindAll<T>(this IRepository<T> repo, CancellationToken cancellationToken)
    {
        return repo.Find(FindAllSpec<T>.Instance, cancellationToken);
    }

    public static async ValueTask<T?> FindSingle<T>(this IRepository<T> repo, ISpecification<T> spec, CancellationToken cancellationToken)
    {
        var results = await repo.Find(spec, cancellationToken);
        
        if (results.Count == 1)
        {
            return results[0];
        }

        return default;
    }
}
