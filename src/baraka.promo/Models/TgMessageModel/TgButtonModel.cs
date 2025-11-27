using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Models.TgMessageModel
{
    public class TgButtonModel
    {
        public string? Name { get; set; }
        [RegularExpression(@"^https://.*$", ErrorMessage = "URL-адрес должен начинаться с https://")]
        public string? Url { get; set; }
    }
}
