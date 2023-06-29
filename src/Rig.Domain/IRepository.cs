namespace Rig.Domain;

public interface IRepository<T>
{
    ValueTask<IReadOnlyList<T>> Find(ISpecification<T> spec, CancellationToken cancellationToken);
    ValueTask Save(IEnumerable<T> item, CancellationToken cancellationToken);
}
