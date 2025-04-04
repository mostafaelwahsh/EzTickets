using EzTickets.DTO.Pagination;

namespace EzTickets.Repository
{
    public interface IRepository<T>
    {
        PagedResponse<T> GetAll(PaginationParams pagination);
        T GetById(int Id);
        void Update(T obj);
        void Insert(T obj);
        void Delete(int Id);
        void Save();
    }
}
