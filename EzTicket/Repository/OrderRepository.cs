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
                             .FirstOrDefault(o => o.OrderId == id);
        }

        public Order LastOrder()
        {
            return   _context.Order
                      .OrderByDescending(o => o.CreatedAt)
                      .FirstOrDefault();
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
            }
        }
        public void IsDeletedTrue(int id)
        {
            var order = GetById(id);
            if (order != null)
            {
                order.IsDeleted = true;
            }
        }

        public List<Order> GetByUserId(string userId)
        {
            return _context.Order
                .AsNoTracking()
                .Where(o => o.UserID == userId && o.IsDeleted == false)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
