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
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required");

            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sub = await _db.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);

            if (sub == null)
                return BadRequest("Subscription not found");

            // 🔥 règle Free
            if (sub.Plan == PlanType.Free)
            {
                var count = await _db.Events.CountAsync(e => e.UserId == userId);

                if (count >= 1)
                    return BadRequest("Upgrade required");
            }

            var slug = await GenerateUniqueSlug(dto.Title);

            var ev = new Event
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                HostName = dto.HostName?.Trim(),
                EventDate = dto.EventDate,
                Slug = slug,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                ev.Id,
                ev.Slug,
                PublicUrl = $"http://localhost:3000/w/{ev.Slug}"
            });
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var ev = await _db.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug);

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

        private async Task<string> GenerateUniqueSlug(string title)
        {
            var baseSlug = title.ToLower().Replace(" ", "-");
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
