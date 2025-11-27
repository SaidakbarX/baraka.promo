using baraka.promo.Data.Base;
using baraka.promo.Models.Enums;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Loyalty
{
    public class Card : Entity
    {
        private Card()
        {

        }
        public Card(string number, Guid user_id, decimal balance, CardType type, string? holder_name, string? product_info, string user) : base(user)
        {
            Id = Guid.NewGuid();
            Number = number;
            UserId = user_id;
            Balance = balance;
            Type = type;
            HolderName = holder_name;
            ProductInfo = product_info;
        }

        public Guid Id { get; private set; }
        [MaxLength(100)]
        public string Number { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public CardType Type { get; private set; }
        [MaxLength(150)]
        public string? HolderName { get; private set; }
        [MaxLength(1000)]
        public string? ProductInfo { get; private set; }
        public long HolderId { get; set; }

        public void SetBalance(decimal balance)
        {
            Balance = balance;
        }
        public void PlusBalance(decimal balance)
        {
            Balance += balance;
        }
        public void MinusBalance(decimal balance)
        {
            Balance -= balance;
        }

        public void Update(CardType type, string user)
        {
            Type = type;

            Modified(user);
        }
    }
}
