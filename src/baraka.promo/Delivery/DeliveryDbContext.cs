using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Delivery
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
           : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }

        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerDevice> CustomerDevices { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {           
            modelBuilder.Entity<OrderType>().HasQueryFilter(item => !item.IsDeleted);
            modelBuilder.Entity<Delivery>().HasQueryFilter(order => order.Status == 201);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(item => !item.IsDeleted && item.Status != 255);
            modelBuilder.Entity<Product>().HasQueryFilter(item => !item.IsDeleted);
            modelBuilder.Entity<ProductCategory>().HasQueryFilter(item => !item.IsDeleted);

           

        }

    }
}