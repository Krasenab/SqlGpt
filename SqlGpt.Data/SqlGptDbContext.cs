using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SqlGpt.Models;

namespace SqlGpt.Data
{
    public class SqlGptDbContext: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public SqlGptDbContext(DbContextOptions<SqlGptDbContext> options)
          : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Chat>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Chats)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
