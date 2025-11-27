using baraka.promo.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Models.LoyaltyApiModels.Cardholders
{
    public class CardholderModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите имя")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Пожалуйста, укажите номер телефона")]
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Пожалуйста, выберите тип держателя карты")]
        public CardholderType Type { get; set; }
        [Required(ErrorMessage = "Пожалуйста, укажите email")]
        public string Email { get; set; }
        public CardholderSex Sex { get; set; }
    }
}
