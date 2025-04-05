using Models;

public class SalesReportDTO
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string VenueName { get; set; }
    public string City { get; set; }
    public EventCategoryType Category { get; set; }
    public EventStatus Status { get; set; }
    public DateTime StartDate { get; set; }

    // Ticket information
    public int TotalTickets { get; set; }
    public int AvailableTickets { get; set; }
    public int SoldTickets => TotalTickets - AvailableTickets;
    public decimal PricePerTicket { get; set; }

    // Sales calculations
    public decimal PotentialTotalSales => TotalTickets * PricePerTicket;
    public decimal TotalSales => SoldTickets * PricePerTicket;
    public decimal ActualTotalSales => SoldTickets * PricePerTicket;
    public decimal OccupancyRate => TotalTickets > 0
        ? (decimal)SoldTickets / TotalTickets * 100
        : 0;
}