using Data;
using EzTickets.DTO.Pagination;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EzTickets.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;

        public OrderRepository(DataContext context)
        {
            _context = context;
        }

        public PagedResponse<Order> GetAll(PaginationParams pagination)
        {
            var query = _context.Order
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.OrderId);

            var pagedData = query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var totalRecords = query.Count();

            return new PagedResponse<Order>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }

        public Order? GetById(int id)
        {
            return _context.Order
                .AsNoTracking()
                .Include(o => o.Tickets)
                .FirstOrDefault(o => o.OrderId == id && o.IsDeleted == false);
        }


        public void Insert(Order order)
        {
            _context.Order.Add(order);
        }

        public void Update(Order order)
        {
            _context.Order.Update(order);
        }

        public void Delete(int id)
        {
            var order = GetById(id);
            if (order != null)
            {
                order.IsDeleted = true;
                _context.SaveChanges();
            }
        }

        public List<Order> GetByUserId(string userId)
        {
            return _context.Order
                .AsNoTracking()
                .Where(o => o.UserID == userId && o.IsDeleted == false)
                .Include(o => o.Tickets)
                .ToList();
        }

        public List<Order> GetExpiringOrders(DateTime targetDate)
        {
            return _context.Order
                .AsNoTracking()
                .Where(o => o.ExpirationDate.HasValue && o.ExpirationDate.Value <= targetDate)
                .Include(o => o.Tickets)
                .ToList();
        }

        public PagedResponse<Order> GetPagedOrders(PaginationParams paginationParams)
        {
            // Get total count of orders
            var totalRecords = _context.Order
                .AsNoTracking()
                .Where(o => o.IsDeleted == false)
                .Count();

            // Fetch the data for the current page
            var orders = _context.Order
                .AsNoTracking()
                .Where(o => o.IsDeleted == false)
                .Include(o => o.Tickets)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToList();

            // Return paged response
            return new PagedResponse<Order>(orders, paginationParams.PageNumber, paginationParams.PageSize, totalRecords);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
