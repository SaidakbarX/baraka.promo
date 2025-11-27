using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models;
using baraka.promo.Models.Cards;
using baraka.promo.Models.Enums;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Services;
using baraka.promo.Utils;
using DocumentFormat.OpenXml.Office.Word;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace baraka.promo.Core.Transactions
{
    public class AddBonusTransaction
    {
        public class Command : IRequest<ApiBaseResultModel<LoyaltyBonusResultModel>>
        {
            public Command(TransactionBonusModel model, string integrationName)
            {
                Model = model;
                IntegrationName = integrationName;
            }

            public TransactionBonusModel Model { get; set; }
            public string IntegrationName { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyBonusResultModel>>
        {
            readonly ILogger<AddBonusTransaction> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<AddBonusTransaction> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<LoyaltyBonusResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Model;

                    _logger.LogWarning($"AddBonusTransaction -> {JsonConvert.SerializeObject(model)}");
                    if (string.IsNullOrEmpty(model.ExternalId)) return new ApiBaseResultModel<LoyaltyBonusResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_MODEL_EMPTY));

                    Guid transaction_id;
                    LoyaltyBonusResultModel result = new LoyaltyBonusResultModel();

                    var card = await _db.Cards.FirstOrDefaultAsync(x => (x.Id.ToString() == model.Card || x.Number == model.Card) && !x.IsDeleted);
                    if (card == null) return new ApiBaseResultModel<LoyaltyBonusResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                    if (!_db.Transactions.Any(x => x.ExternalId == model.ExternalId && x.Status == TransactionStatus.Success))
                    {
                        if (model.BonusSum > 0 && card.Balance < model.BonusSum) return new ApiBaseResultModel<LoyaltyBonusResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INVALID_CARD_BALANCE));

                        decimal sum_for_cashback = model.Sum - model.BonusSum;

                        BonusHistory bonusHistory = new BonusHistory
                        {
                            TotalSum = model.Sum,
                            BonusUsed = model.BonusSum,
                            CreatedAt = DateTime.Now,
                        };

                        // discounts to order
                        var cashback = await _db.LoyalityTypes.FirstOrDefaultAsync(x => x.Type == LoyalityTypeKey.CASHBACK.ToString() && x.IsActive);
                        if (cashback != null)
                        {
                            var cashback_infos = JsonConvert.DeserializeObject<List<CashbackModel>>(cashback.ValueInfo);
                            var card_cashback = cashback_infos.FirstOrDefault(x => x.CardType == card.Type);
                            if (card_cashback != null)
                            {
                                decimal cashback_sum = sum_for_cashback * card_cashback.Value / 100;

                                // Начисляем бонусы-кэшбэк
                                card.PlusBalance(cashback_sum);

                                bonusHistory.CashbackId = cashback.Id;
                                bonusHistory.CashbackSum = cashback_sum;

                                result.Cashback = cashback_sum;
                            }
                        }

                        // Списываем бонусы с карты
                        if (model.BonusSum > 0) card.MinusBalance(model.BonusSum);

                        Transaction transaction = new Transaction(card.Id, card.Number, card.Balance, model.Sum, TransactionType.WriteOff, request.IntegrationName,
                                                                 TransactionStatus.Success, model.ExternalId, model.ExternalData, DateTime.Now);

                        await _db.Transactions.AddAsync(transaction, cancellationToken);
                        transaction_id = transaction.Id;

                        bonusHistory.TransactionId = transaction_id;
                        await _db.BonusHistories.AddAsync(bonusHistory, cancellationToken);

                        await _db.SaveChangesAsync(cancellationToken);

                        var cache_key = CardInfoModel.CACHE_KEY + card.Number;
                        _memory_cache.Remove(cache_key);

                        var cache_key_bonus = TransactionBonusInfoModel.CACHE_KEY + card.Id;
                        _memory_cache.Remove(cache_key_bonus);

                        var cache_key_user = UserCardInfoModel.CACHE_KEY + card.UserId;
                        _memory_cache.Remove(cache_key_user);
                    }
                    else transaction_id = _db.Transactions.FirstOrDefault(x => x.ExternalId == model.ExternalId).Id;

                    result.Id = transaction_id;
                    _logger.LogWarning($"AddBonusTransaction result -> {JsonConvert.SerializeObject(result)}");

                    return new ApiBaseResultModel<LoyaltyBonusResultModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<LoyaltyBonusResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
