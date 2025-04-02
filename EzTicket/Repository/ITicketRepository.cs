using Models;
using EzTickets.DTO;

namespace EzTickets.Repository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        // Get by string ID (matching your model)
        Ticket GetById(string id);
        void DeleteById(string id);

        // Additional query methods
        List<Ticket> GetTicketsByEventId(string eventId);
        List<Ticket> GetTicketsByUserId(string userId);
        List<Ticket> GetTicketsByStatus(TicketStatus status);
        List<Ticket> GetTicketsByType(TicketType type);

        // Specialized update methods
        bool UpdateTicketStatus(string ticketId, TicketStatus status);
        bool AssignTicketToUser(string ticketId, string userId);

        // Soft delete functionality
       

        // Statistics methods
        int GetAvailableTicketsCountByEvent(string eventId);
        decimal GetTotalSalesByEvent(string eventId);

        // Generate QR code for ticket
        string GenerateQRCode(string ticketId);
    }
}