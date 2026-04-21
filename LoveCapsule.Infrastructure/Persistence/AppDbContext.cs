using Microsoft.EntityFrameworkCore;
using LoveCapsule.Domain.Entities;
using System.Collections.Generic;

namespace LoveCapsule.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👤 User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Email)
                      .IsUnique();
            });

            // 💳 Subscription
            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Plan)
                      .HasConversion<string>(); // enum → string

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🎉 Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Slug)
                      .IsUnique();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(x => x.UserId);
            });

            // 💬 Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(m => m.Event)
                      .WithMany(e => e.Messages)
                      .HasForeignKey(m => m.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}