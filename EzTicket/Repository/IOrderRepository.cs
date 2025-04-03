using Data;
using EzTickets.DTO.Pagination;
using Models;

namespace EzTickets.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {

        List<Order> GetByUserId(string userId);


        List<Order> GetExpiringOrders(DateTime targetDate);


        PagedResponse<Order> GetPagedOrders(PaginationParams paginationParams);


    }
}
