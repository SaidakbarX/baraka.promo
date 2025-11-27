namespace baraka.promo.Models
{
    public class ListBaseModel<T>
    {
        public int Total { get; set; }
        public List<T> List { get; set; } = new();
    }
}
