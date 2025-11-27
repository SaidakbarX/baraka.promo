namespace baraka.promo.Models.Paging
{
    public sealed class PageResultModel<T>
    {
        public int TotalCount { get; set; }
        public List<T> Value { get; set; } = new List<T>();
    }
}