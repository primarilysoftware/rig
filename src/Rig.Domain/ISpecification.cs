namespace Rig.Domain;

public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> queryable);
}
