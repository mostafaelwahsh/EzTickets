using Data;
using EzTickets.DTO.Pagination;
using Models;

namespace EzTickets.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {

        List<Order> GetByUserId(string userId);

        void IsDeletedTrue(int id);

        Order LastOrder();

        PagedResponse<Order> GetPagedOrders(PaginationParams paginationParams);


    }
}
