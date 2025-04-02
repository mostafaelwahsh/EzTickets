using Data;
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

        public List<Order> GetAll()
        {
            return _context.Order
                .Include(o => o.Tickets)
                .ToList();
        }

        public Order GetById(int id)
        {
            return _context.Order
                .Include(o => o.Tickets)
                .FirstOrDefault(o => o.OrderID == id.ToString());
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
                _context.Order.Remove(order);
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
