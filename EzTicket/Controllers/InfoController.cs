using EzTickets.DTO.Public;
using EzTickets.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IInfoRepository _infoRepository;

        public InfoController(IInfoRepository infoRepository)
        {
            _infoRepository = infoRepository;
        }
        [HttpGet("about")]
        public ActionResult<GeneralResponse> GetAboutInfo()
        {
            return (new GeneralResponse
            {
                IsPass = true,
                Data = new
                {
                    AppName = "EzTickets",
                    Version = "1.0.0",
                    Description = "Online ticketing platform for events",
                    Company = "EzTickets Inc.",
                    Year = 2025
                }
            });
        }

        [HttpGet("contact")]
        public ActionResult<GeneralResponse> GetContactInfo()
        {
            return (new GeneralResponse
            {
                IsPass = true,
                Data = new
                {
                    Email = "support@eztickets.com",
                    Phone = "+20 101 762 4010",
                    Address = "Information Technology Institute, Smart Village, Giza",
                    WorkingHours = "Sat-Thu 9AM-7PM"
                }
            });
        }


        [HttpPost("contact")]
        public async Task<ActionResult<GeneralResponse>> SubmitContactRequest(
            [FromBody] ContactRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return (new GeneralResponse
                {
                    IsPass = false,
                    Data = ModelState
                });
            }

            await _infoRepository.SubmitContactRequestAsync(request);

            return (new GeneralResponse
            {
                IsPass = true,
                Data = "Thank you for contacting us! We'll respond within 24 hours."
            });
        }

        [HttpGet]
        public ActionResult<GeneralResponse> GetFAQ()
        {
            var faqs = new List<FAQItemDTO>()
            {
                  new FAQItemDTO
                {
                    Question = "How do I purchase tickets?",
                    Answer = "Select an event, choose tickets, and proceed to checkout."
                },
                new FAQItemDTO
                {
                    Question = "Can I get a refund?",
                    Answer = "Refunds are available up to 7 days before the event."
                }
                ,
                new FAQItemDTO
                {
                    Question = "What payment methods do you accept?",
                    Answer = "We accept all major credit cards, PayPal, and select mobile payment options."
                },
                new FAQItemDTO
                {
                    Question = "How do I contact customer support?",
                    Answer = "Use our contact form or email support@eztickets.com. We respond within 24 hours."
                },
            };
            return (new GeneralResponse
            {
                IsPass = true,
                Data = faqs
            });
        }
    }
}
