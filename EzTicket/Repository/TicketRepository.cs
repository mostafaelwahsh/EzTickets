using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using EzTickets.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzTickets.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context)
        {
            _context = context;
        }

        public List<Ticket> GetAll()
        {
            return _context.Ticket
                .Where(t => !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .ToList();
        }

        // Original method kept for interface compatibility
        public Ticket GetById(int Id)
        {
            throw new NotImplementedException("Use string ID version instead");
        }

        
        public Ticket GetById(string id)
        {
            return _context.Ticket
                .Include(t => t.Event) 
                .Include(t => t.User)   
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

        // Delete is now implemented with the ID as string
        public void Delete(int Id)
        {
            throw new NotImplementedException("Use string ID version instead");
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

        // Soft delete functionality
        public bool SoftDelete(string ticketId)
        {
            var ticket = GetById(ticketId);
            if (ticket == null) return false;

            ticket.IsDeleted = true;
            Update(ticket);
            return Save() > 0;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        // Additional query methods
        public List<Ticket> GetTicketsByEventId(int eventId)
        {
            return _context.Ticket
                .Where(t => t.EventID == eventId && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .ToList();
        }

        public List<Ticket> GetTicketsByUserId(string userId)
        {
            return _context.Ticket
                .Where(t => t.UserID == userId && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .ToList();
        }

        public List<Ticket> GetTicketsByStatus(TicketStatus status)
        {
            return _context.Ticket
                .Where(t => t.TicketStatus == status && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .ToList();
        }

        public List<Ticket> GetTicketsByType(TicketType type)
        {
            return _context.Ticket
                .Where(t => t.TicketType == type && !t.IsDeleted)
                .Include(t => t.Event)
                .Include(t => t.User)
                .ToList();
        }

        // Specialized update methods
        public bool UpdateTicketStatus(string ticketId, TicketStatus status)
        {
            var ticket = GetById(ticketId);
            if (ticket == null) return false;

            ticket.TicketStatus = status;
            Update(ticket);
            return Save() > 0;
        }

        public bool AssignTicketToUser(string ticketId, string userId)
        {
            var ticket = GetById(ticketId);
            if (ticket == null) return false;

            ticket.UserID = userId;
            ticket.TicketStatus = TicketStatus.Available; // Changed to Available since SoldOut isn't for individual tickets
            ticket.PurchaseDate = DateTime.UtcNow;
            Update(ticket);
            return Save() > 0;
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