using System.ComponentModel.DataAnnotations.Schema;

namespace baraka.promo.Delivery
{
    public class OrderItem
    {
        public Guid Id { get; private set; }
        [ForeignKey("Order")]
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int Status { get; private set; }
        public bool IsDeleted { get; private set; }
    }
}