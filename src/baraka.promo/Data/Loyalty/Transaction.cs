using baraka.promo.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace baraka.promo.Data.Loyalty
{
    [Index(nameof(CardId))]
    [Index(nameof(ExternalId))]

    public class Transaction
    {
        private Transaction()
        {

        }
        public Transaction(Guid card_id, string card_number, decimal balance, decimal sum, TransactionType type, string user, 
                           TransactionStatus status,
                           string? external_id, string? external_data, DateTime? finished_time)
        {
            CardId = card_id;
            CardNumber = card_number;
            Sum = sum;
            Balance = balance;
            Type = type;
            CreatedBy = user;
            CreatedTime = DateTime.Now;
            Status = status;
            ExternalId = external_id;
            ExternalData = external_data;
            FinishedTime = finished_time;
        }

        public Guid Id { get; private set; }
        public Guid CardId { get; private set; }
        [MaxLength(100)]
        public string CardNumber { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Sum { get; private set; }
        //public decimal BonusSum { get; private set; }
        [MaxLength(100)]
        public string? ExternalId { get; private set; }
        public string? ExternalData { get; private set; }
        public TransactionType Type { get; private set; }
        [MaxLength(256)]
        public string CreatedBy { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public TransactionStatus Status { get; private set; }
        public DateTime? FinishedTime { get; private set; }
        public DateTime? CanceledTime { get; private set; }

        public void SetFinishedTime(DateTime finished_time)
        {
            FinishedTime = finished_time;
        }
        public void SetCanceledTime(DateTime canceled_time)
        {
            CanceledTime = canceled_time;
        }
        public void SetStatus(TransactionStatus status)
        {
            Status = status;
        }
        public void SetExternalId(string external_id)
        {
            ExternalId = external_id;
        }
    }
}
