using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using EzTickets.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using EzTickets.DTO.Pagination;

namespace EzTickets.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context)
        {
            _context = context;
        }

        public List<Ticket> GetAll(PaginationParams pagination)
        {
            return _context.Ticket
                .Where(t => !t.IsDeleted)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Include(t => t.Event)
                .Include(t => t.User)
                .Include(t => t.Order)
                .ToList();
        }

        
        public Ticket GetById(string id)
        {
            return _context.Ticket
                .Include(t => t.Event) 
                .Include(t => t.User)   
                .Include(t=>t.Order)
                .FirstOrDefault(t => t.TicketID == id);
        }

        public void Insert(Ticket ticket)
        {
            // Set default values if not provided
            if (string.IsNullOrEmpty(ticket.TicketID))
                ticket.TicketID = Guid.NewGuid().ToString();

            if (ticket.CreatedAt == default)
                ticket.CreatedAt = DateTime.UtcNow;

            _context.Ticket.Add(ticket);
            
        }

        public void Update(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
        }

        // Delete by string ID
        public void DeleteById(string id)
        {
            var ticket = GetById(id);
            if (ticket != null)
            {
                _context.Ticket.Remove(ticket);
            }
        }

        public void UpdateRangeofTickets(List<Ticket> availableTickets)
        {
            _context.Ticket.UpdateRange(availableTickets);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        // Additional query methods
        public List<Ticket> GetTicketsByEventId(int eventId)
        {
            return _context.Ticket
                .Where(t => t.EventID == eventId && !t.IsDeleted && t.TicketStatus==TicketStatus.Available).ToList();
                
        }

        public List<Ticket> GetTicketsByUserId(string userId)
        {
            return _context.Ticket
                .Where(t => t.UserID == userId && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .Include(t => t.Order)
                .ToList();
        }

        public List<Ticket> GetTicketsByStatus(TicketStatus status)
        {
            return _context.Ticket
                .Where(t => t.TicketStatus == status && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .Include(t => t.Order)
                .ToList();
        }

        public List<Ticket> GetTicketsByType(TicketType type)
        {
            return _context.Ticket
                .Where(t => t.TicketType == type && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                 .Include(t => t.Order)
                .ToList();
        }

        // Specialized update methods
        public bool UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = GetById(ticketId);
            if (ticket == null) return false;

            ticket.TicketStatus = status;
            Update(ticket);
            Save();
            return true;
        }

        public bool AssignTicketToUser(string ticketId, string userId)
        {
            var ticket = GetById(ticketId);
            if (ticket == null) return false;

            ticket.UserID = userId;
            ticket.TicketStatus = TicketStatus.Available; // Changed to Available since SoldOut isn't for individual tickets
            ticket.PurchaseDate = DateTime.UtcNow;
            Update(ticket);
            Save();
            return true;
        }

        // Statistics methods
        public int GetAvailableTicketsCountByEvent(int eventId)
        {
            return _context.Ticket
                .Count(t => t.EventID == eventId &&
                           t.TicketStatus == TicketStatus.Available &&
                           !t.IsDeleted);
        }

        public decimal GetTotalSalesByEvent(int eventId)
        {
            return _context.Ticket
                .Where(t => t.EventID == eventId &&
                           t.UserID != null &&
                           !t.IsDeleted)
                .Sum(t => t.Price);
        }

        // Generate QR code for ticket
        public string GenerateQRCode(string ticketId)
        {
            // In a real implementation, you would use a library to generate QR codes
            var ticket = GetById(ticketId);
            if (ticket == null) return null;

            // Create a QR code with ticket information
            string qrData = $"TICKET:{ticket.TicketID}|EVENT:{ticket.EventID}|USER:{ticket.UserID ?? "UNASSIGNED"}|SEAT:{ticket.SeatNumber ?? 0}";

            // Update the ticket with the QR code data
            ticket.QRCode = qrData;
            Update(ticket);
            Save();

            return qrData;
        }
    }
}