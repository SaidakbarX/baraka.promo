using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data;
using baraka.promo.Models.Enums;
using baraka.promo.Services;
using baraka.promo.Utils;
using baraka.promo.Data.Loyalty;

namespace baraka.promo.Core.Transactions
{
    public class ReplenishBalance
    {
        public class Command : IRequest<ApiBaseResultModel<LoyaltyResultModel>>
        {
            public Command(ReplenishmentModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public ReplenishmentModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyResultModel>>
        {
            readonly ILogger<ReplenishBalance> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<ReplenishBalance> logger, ApplicationDbContext db, ICurrentUser currentUser)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
            }

            public async Task<ApiBaseResultModel<LoyaltyResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    var card = _db.Cards.FirstOrDefault(x => x.Number == model.CardNumber && !x.IsDeleted);
                    if (card == null) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                    Transaction transaction = new Transaction(card.Id, card.Number, card.Balance, model.Sum, TransactionType.Replenishment, user,
                                                             TransactionStatus.Success, null, null, DateTime.Now);
                    await _db.Transactions.AddAsync(transaction, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    card.PlusBalance(model.Sum);
                    await _db.SaveChangesAsync(cancellationToken);

                    return new ApiBaseResultModel<LoyaltyResultModel>(new LoyaltyResultModel { Id = transaction.Id });
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
