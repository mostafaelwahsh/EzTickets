using Models;

namespace EzTickets.Repository
{
    public interface IEventRepository : IRepository<Event>
    {

        List<Event> GetEventsByCity(string city);
        List<Event> GetEventsByCountry(string country);
        List<Event> GetEventsByDate(DateTime date);
        List<Event> GetEventsByCategory(EventCategoryType category);
        List<Event> GetEventsByStatus(EventStatus status);
        List<Event> GetEventsByDateRange(DateTime startDate, DateTime endDate);
        List<Event> GetEventsByPriceRange(decimal minPrice, decimal maxPrice);
        List<Event> GetEventsByVenue(string venueName);
        List<Event> GetEventsByName(string eventName);
        void SoftDeleteEvent(int eventId);
        void RestoreEvent(int eventId);
        int GetTotalEventsCount();
    }
}
