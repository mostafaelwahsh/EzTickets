namespace EzTickets.Repository
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T GetById(int Id);
        void Update(T obj);
        void Insert(T obj);
        void Delete(int Id);
        int Save();
    }
}
