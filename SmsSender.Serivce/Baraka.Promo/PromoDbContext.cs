using Microsoft.EntityFrameworkCore;
using SmsSender.Serivce.Data;
using SmsSender.Serivce.Models;

namespace SmsSender.Serivce.Baraka.Promo
{
    public class PromoDbContext : DbContext
    {
        public PromoDbContext(DbContextOptions<PromoDbContext> options) : base(options)
        {   
        }

        public DbSet<MessageHeader> MessageHeaders { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NotificationHeader> NotificationHeaders { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Message>()
            .HasKey(c => new { c.ChatId, c.MessageHeaderId });
            builder.Entity<Notification>()
           .HasKey(c => new { c.DeviceId, c.NotificationHeaderId });
            base.OnModelCreating(builder);
        }
    }
}
