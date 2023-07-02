namespace CS.Data.Base
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Get();

        T GetById(int id);

        void Add(T item);

        Task Add(IEnumerable<T> items);

        T AddAndReturn(T item);

        void Update(T item);

        void Remove(T item);
    }
}