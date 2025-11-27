using baraka.promo.Data.Loyalty;
using baraka.promo.Data.PromoEntities;
using baraka.promo.Data.Subscriptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Promo> Promos { get; set; }
        public DbSet<PromoClient> PromoClients { get; set; }
        public DbSet<PromoProduct> PromoProducts { get; set; }
        public DbSet<PromoProductFeature> PromoProductFeatures { get; set; }
        public DbSet<PromoRegion> PromoRegions { get; set; }
        public DbSet<PromoRestaurant> PromoRestaurants { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<MessageHeader> MessageHeaders { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageTime> MessageTimes { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Cardholder> Cardholders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PromoGroup> PromoGroups { get; set; }
        public DbSet<PromoArbitration> PromoArbitrations { get; set; }
        public DbSet<PromoChildValue> PromoChildValues { get; set; }
        public DbSet<Integration> Integrations { get; set; }
        public DbSet<PromoIntegration> PromoIntegrations { get; set; }
        public DbSet<LoyalityType> LoyalityTypes { get; set; }
        public DbSet<BonusHistory> BonusHistories { get; set; }
        public DbSet<CardToken> CardTokens { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionClient> SubscriptionClients { get; set; }
        public DbSet<SubscriptionGroup> SubscriptionGroups { get; set; }
        public DbSet<NotificationHeader> NotificationHeaders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationTime> NotificationTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PromoProduct>()
           .HasKey(c => new { c.ProductId, c.PromoId });

            builder.Entity<PromoProductFeature>()
           .HasKey(c => new { c.ProductId, c.PromoId });

            builder.Entity<PromoRegion>()
           .HasKey(c => new { c.RegionId, c.PromoId });

            builder.Entity<PromoRestaurant>()
           .HasKey(c => new { c.RestaurantId, c.PromoId });

            builder.Entity<Message>()
            .HasKey(c => new { c.ChatId, c.MessageHeaderId });
            
            builder.Entity<MessageTime>()
            .HasKey(c => new { c.Id, c.MessageHeaderId });

            builder.Entity<Promo>()
                .Property(a => a.IsActive)
                .HasDefaultValue(true);

            //builder.Entity<MessageHeader>()
            //    .Property(a => a.IsActive)
            //    .HasDefaultValue(true);

            builder.Entity<Card>()
            .HasKey(c => new { c.Id, c.Number, c.UserId });

            builder.Entity<PromoArbitration>()
            .HasKey(c => new { c.ProductId, c.PromoId });

            builder.Entity<PromoIntegration>()
            .HasKey(c => new { c.PromoId, c.IntegrationId });

            builder.Entity<BonusHistory>()
            .HasKey(c => new { c.TransactionId, c.CashbackId, c.CreatedAt });
            builder.Entity<Notification>()
           .HasKey(c => new { c.DeviceId, c.NotificationHeaderId });

            builder.Entity<NotificationTime>()
            .HasKey(c => new { c.Id, c.NotificationHeaderId });
            builder.Entity<SubscriptionClient>()
           .HasKey(c => new { c.Id, c.SubscriptionId });


            base.OnModelCreating(builder);
        }
    }
}