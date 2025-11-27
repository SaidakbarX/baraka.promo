using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.Enums;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Core.Transactions
{
    public class CancelBonusTransaction
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(string external_id)
            {
                ExternalId = external_id;
            }

            public string ExternalId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<CancelBonusTransaction> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<CancelBonusTransaction> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    string external_id = request.ExternalId;

                    _logger.LogWarning($"CancelBonusTransaction -> {external_id}");

                    var transactions = await _db.Transactions.Where(x => x.ExternalId == external_id).ToListAsync();
                    if (transactions?.Count == 0) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_TRANSACTION_NOT_FOUND));

                    foreach (var transaction in transactions)
                    {
                        if (transaction.Status == TransactionStatus.Success)
                        {
                            transaction.SetStatus(TransactionStatus.Cancelled);
                            transaction.SetCanceledTime(DateTime.Now);

                            var bonus_history = await _db.BonusHistories.FirstOrDefaultAsync(x => x.TransactionId == transaction.Id);
                            if (bonus_history != null)
                            {
                                var card = await _db.Cards.FirstOrDefaultAsync(x => x.Id == transaction.CardId);

                                card.MinusBalance(bonus_history.CashbackSum);
                                card.PlusBalance(bonus_history.BonusUsed);
                            }

                            await _db.SaveChangesAsync(cancellationToken);
                        }
                    }

                    _logger.LogWarning($"CancelBonusTransaction result -> OK -> {external_id}");

                    return new ApiBaseResultModel();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
