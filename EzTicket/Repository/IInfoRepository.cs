using EzTickets.DTO.Public;
using Models;

namespace EzTickets.Repository
{
    public interface IInfoRepository
    {
        Task SubmitContactRequestAsync(ContactRequestDTO request);
        Task<List<ContactRequest>> GetAllRequests();

    }
}
