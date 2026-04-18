using Microsoft.AspNetCore.Mvc;
using LoveCapsule.Infrastructure.Persistence;
using LoveCapsule.Domain.Entities;
using LoveCapsule.Api.DTOs;

namespace LoveCapsule.Api.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email required");

            var slug = GenerateSlug(dto.Title);

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                HostName = dto.HostName,
                Email = dto.Email,
                EventDate = dto.EventDate,
                Slug = slug,
                CreatedAt = DateTime.UtcNow
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                ev.Id,
                ev.Slug,
                PublicUrl = $"http://localhost:3000/w/{slug}"
            });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var ev = _db.Events.FirstOrDefault(x => x.Slug == slug);

            if (ev == null)
                return NotFound();

            return Ok(new
            {
                ev.Id,
                ev.Title,
                ev.HostName,
                ev.EventDate
            });
        }

        private string GenerateSlug(string title)
        {
            return title.ToLower().Replace(" ", "-");
        }
    }
}
