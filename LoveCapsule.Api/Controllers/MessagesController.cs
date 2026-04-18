using Microsoft.AspNetCore.Mvc;
using LoveCapsule.Infrastructure.Persistence;
using LoveCapsule.Domain.Entities;
using LoveCapsule.Api.DTOs;
using LoveCapsule.Api.Services;

namespace LoveCapsule.Api.Controllers
{

    [ApiController]
    [Route("messages")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MessagesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMessageDto dto)
        {
            if (dto.EventId == Guid.Empty)
                return BadRequest("EventId required");

            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Message required");

            if (dto.Text.Length > 500)
                return BadRequest("Message too long");

            var ev = await _db.Events.FindAsync(dto.EventId);

            if (ev == null)
                return NotFound("Event not found");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                EventId = dto.EventId,
                Text = dto.Text,
                AuthorName = dto.AuthorName,
                CreatedAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetByEvent(Guid eventId)
        {
            var messages = _db.Messages
                .Where(x => x.EventId == eventId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.Text,
                    x.AuthorName,
                    x.CreatedAt
                })
                .ToList();

            return Ok(messages);
        }

        [HttpGet("book/{eventId}")]
        public async Task<IActionResult> GetBook(Guid eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);

            if (ev == null)
                return NotFound("Event not found");

            var messages = _db.Messages
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Text,
                    AuthorName = string.IsNullOrWhiteSpace(x.AuthorName)
                        ? "Un invité"
                        : x.AuthorName,
                    x.CreatedAt
                })
                .ToList();

            var result = new
            {
                Title = ev.Title,
                HostName = ev.HostName,
                EventDate = ev.EventDate,
                Messages = messages
            };

            return Ok(result);
        }
        [HttpGet("book/pdf/{eventId}")]
        public async Task<IActionResult> GetBookPdf(Guid eventId, [FromServices] PdfService pdfService)
        {
            var ev = await _db.Events.FindAsync(eventId);

            if (ev == null)
                return NotFound();

            var messages = _db.Messages
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.CreatedAt)
                .ToList();

            var formatted = messages.Select(x => (
                x.Text,
                string.IsNullOrWhiteSpace(x.AuthorName) ? "Un invité" : x.AuthorName
            )).ToList();

            var pdf = pdfService.GenerateBook(
                ev.Title,
                ev.HostName,
                ev.EventDate,
                formatted
            );

            return File(pdf, "application/pdf", "livre-souvenir.pdf");
        }
    }
}
