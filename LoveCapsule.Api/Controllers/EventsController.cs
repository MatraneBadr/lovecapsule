using Microsoft.AspNetCore.Mvc;
using LoveCapsule.Infrastructure.Persistence;
using LoveCapsule.Domain.Entities;
using LoveCapsule.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload");

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // Génération slug unique
            var baseSlug = GenerateSlug(dto.Title);
            var slug = await GenerateUniqueSlug(baseSlug);

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                HostName = dto.HostName?.Trim(),
                EventDate = dto.EventDate,
                Slug = slug,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                IsPaid = false
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBySlug), new { slug = ev.Slug }, new
            {
                ev.Id,
                ev.Slug,
                ev.IsPaid,
                PublicUrl = $"http://localhost:3000/w/{ev.Slug}"
            });
            /*
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
            });*/
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
        private async Task<string> GenerateUniqueSlug(string baseSlug)
        {
            var slug = baseSlug;
            int i = 1;

            while (await _db.Events.AnyAsync(e => e.Slug == slug))
            {
                slug = $"{baseSlug}-{i}";
                i++;
            }

            return slug;
        }

    }
}
