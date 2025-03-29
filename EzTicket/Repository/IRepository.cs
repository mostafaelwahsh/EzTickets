namespace EzTickets.Repository
{
    public interface ICategoryRepository<T>
    {
        List<T> GetAll();
        T GetById(int Id);
        void Update(T obj);
        void Insert(T obj);
        void Delete(int Id);
        int Save();
    }
}
