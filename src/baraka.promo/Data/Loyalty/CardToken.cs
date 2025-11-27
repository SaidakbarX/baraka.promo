using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Loyalty
{
    public class CardToken
    {
        private CardToken()
        {
            
        }

        public CardToken(Guid card_id, string token, DateTime created_time, DateTime expires_time, DateTime reserved_until)
        {
            CardId = card_id;
            Token = token;
            CreatedAt = created_time;
            ExpiresAt = expires_time;
            ReservedUntil = reserved_until;
        }

        public Guid Id { get; private set; }
        public Guid CardId { get; private set; }

        [MaxLength(50)]
        public string Token { get; private set; } // 9 цифр
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }   // +3 минуты
        public DateTime ReservedUntil { get; private set; } // +5 часов
    }
}
