using LoveCapsule.Api.DTOs;
using LoveCapsule.Domain.Entities;
using LoveCapsule.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;

namespace LoveCapsule.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);

            // 🔥 abonnement FREE automatique
            _db.Subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Plan = PlanType.Free,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            var token = GenerateJwt(user);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = Guid.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _db.Users.FindAsync(userId);
            var sub = await _db.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId);

            return Ok(new
            {
                user.Email,
                Plan = sub?.Plan
            });
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_MY_SUPER_SECRET_KEY_123456789"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
