using Data;
using EzTickets.DTO.Public;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EzTickets.Repository
{
    public class InfoRepository : IInfoRepository
    {
        private readonly DataContext _context;
        public InfoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task SubmitContactRequestAsync(ContactRequestDTO request)
        {
            var contactRequest = new ContactRequest
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message,
                SubmittedAt = DateTime.UtcNow,
                Status = ContactStatus.New
            };

            _context.ContactRequests.Add(contactRequest);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ContactRequest>> GetAllRequests()
        {
            return await _context.ContactRequests.ToListAsync();
        }

    }
}
