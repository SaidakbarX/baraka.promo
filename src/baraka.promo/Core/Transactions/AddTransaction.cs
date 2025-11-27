using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;
using baraka.promo.Models.Enums;
using Newtonsoft.Json;

namespace baraka.promo.Core.Transactions
{
    public class AddTransaction
    {
        public class Command : IRequest<ApiBaseResultModel<LoyaltyResultModel>>
        {
            public Command(TransactionModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public TransactionModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyResultModel>>
        {
            readonly ILogger<AddTransaction> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<AddTransaction> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<LoyaltyResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if(user == null) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;
                    _logger.LogWarning($"AddTransaction -> {JsonConvert.SerializeObject(model)}");
                    if (string.IsNullOrEmpty(model.ExternalId)) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_MODEL_EMPTY));

                    Guid transaction_id;

                    if (!_db.Transactions.Any(x=>x.ExternalId == model.ExternalId))
                    {
                        var card = _db.Cards.FirstOrDefault(x => x.Number == model.CardNumber && !x.IsDeleted);
                        if (card == null) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                        // discount to order
                        //{
                            
                        //}
                        
                        if(card.Balance < model.Sum) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INVALID_CARD_BALANCE));

                        Transaction transaction = new Transaction(card.Id, card.Number, card.Balance, model.Sum, TransactionType.WriteOff, user,
                                                                 TransactionStatus.Success, model.ExternalId, model.ExternalData, DateTime.Now);
                        await _db.Transactions.AddAsync(transaction, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        transaction_id = transaction.Id;

                        card.MinusBalance(model.Sum);
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                    else transaction_id = _db.Transactions.FirstOrDefault(x => x.ExternalId == model.ExternalId).Id;

                    LoyaltyResultModel result = new LoyaltyResultModel { Id = transaction_id };
                    _logger.LogWarning($"AddTransaction result -> {JsonConvert.SerializeObject(result)}");

                    return new ApiBaseResultModel<LoyaltyResultModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
