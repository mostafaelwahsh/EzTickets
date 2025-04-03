using Models;
using EzTickets.DTO;

namespace EzTickets.Repository
{
    public interface ITicketRepository 
    {
        // Get by string ID (matching your model)
        List<Ticket> GetAll();
        void Insert(Ticket obj);
        void Save();
        void Update (Ticket obj);
        Ticket GetById(string id);
        void DeleteById(string id);

        // Additional query methods
        List<Ticket> GetTicketsByEventId(int eventId);
        List<Ticket> GetTicketsByUserId(string userId);
        List<Ticket> GetTicketsByStatus(TicketStatus status);
        List<Ticket> GetTicketsByType(TicketType type);

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