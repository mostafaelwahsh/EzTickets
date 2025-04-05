using EzTickets.DTO.Admin;
using Models;

namespace EzTickets.Repository
{
    public interface IInfoRepository
    {
        Task SubmitContactRequestAsync(ContactRequestDTO request);
        Task<List<ContactRequest>> GetAllRequests();

    }
}
