namespace baraka.promo.Delivery
{
    public class OrderType
    {
        public int Id { get; private set; }

        public string? Name_Ru { get; private set; }
        public string? Name_Uz { get; private set; }
        public string? Name_En { get; private set; }
        public string? Icon { get; private set; }

        public bool IsDeleted { get; private set; }
    }
}