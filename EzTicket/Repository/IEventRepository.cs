using EzTickets.DTO.Pagination;
using Models;

namespace EzTickets.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        PagedResponse<Event> GetAllPublic(PaginationParams pagination);
        Event GetByIdPublic(int Id);
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
        PagedResponse<Event> GetFilteredPublicEvents(
        string? searchQuery = null,
        EventCategoryType? category = null,
        string? city = null,
        string? venue = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        PaginationParams? pagination = null);
        Task<bool> PublishEvent(int eventId);
    }
}
