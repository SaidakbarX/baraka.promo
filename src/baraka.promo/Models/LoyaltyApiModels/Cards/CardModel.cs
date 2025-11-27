using baraka.promo.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Models.Cards
{
    public class CardModel
    {
        public string? Number { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public CardType Type { get; set; }
        public string? HolderName { get; set; }
        public string? ProductInfo { get; set; }
    }

    public class CardBalanceModel
    {
        public string Number { get; set; }
        public decimal Balance { get; set; }
    }

    public class UserCardModel
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public CardholderSex Sex { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class UserCardInfoModel
    {
        public UserCardModel User { get; set; }
        public List<CardDetailsModel> Cards { get; set; }
        public const string CACHE_KEY = "UserCardsInfo_CACHE_KEY=";
    }

    public class CardDetailsModel
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; }
        public string? HolderName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
