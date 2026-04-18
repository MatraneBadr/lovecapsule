using Microsoft.EntityFrameworkCore;
using LoveCapsule.Domain.Entities;
using System.Collections.Generic;

namespace LoveCapsule.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Media> Medias => Set<Media>();
        public DbSet<MagicLinkToken> Tokens => Set<MagicLinkToken>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}