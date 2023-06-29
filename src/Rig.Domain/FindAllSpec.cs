namespace Rig.Domain;

public class FindAllSpec<T> : ISpecification<T>
{
    private FindAllSpec() { }

    public IQueryable<T> Apply(IQueryable<T> queryable) => queryable;

    public static FindAllSpec<T> Instance { get; } = new FindAllSpec<T>();
}