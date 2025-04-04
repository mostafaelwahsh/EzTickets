using EzTickets.DTO;
using EzTickets.DTO.Pagination;
using Models;

namespace EzTickets.Repository
{
    public interface ITicketRepository
    {
        // Get by string ID (matching your model)
        List<Ticket> GetAll(PaginationParams pagination);
        void Insert(Ticket obj);
        void Save();
        void Update(Ticket obj);
        Ticket GetById(string id);
        void DeleteById(string id);
        void UpdateRangeofTickets(List<Ticket> availableTickets);

        // Additional query methods
        List<Ticket> GetTicketsByEventId(int eventId);
        List<Ticket> GetTicketsByUserId(string userId);
        List<Ticket> GetTicketsByStatus(TicketStatus status);
        List<Ticket> GetTicketsByType(TicketType type);
        List<Ticket> GetAllTicketsByOrderID(int orderId);

        // Specialized update methods
        bool UpdateTicketStatus(string ticketId, TicketStatus status);
        bool AssignTicketToUser(string ticketId, string userId);

        // Statistics methods
        int GetAvailableTicketsCountByEvent(int eventId);
        decimal GetTotalSalesByEvent(int eventId);

        // Generate QR code for ticket
        string GenerateQRCode(string ticketId);

    }
}