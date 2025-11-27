using baraka.promo.Models.LoyaltyApiModels.Transactions;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.Cards;
using baraka.promo.Core.Transactions;
using baraka.promo.Data;
using baraka.promo.Models.Enums;
using baraka.promo.Services;
using baraka.promo.Utils;

namespace baraka.promo.Core.Cards
{
    public class EditBalance
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(CardBalanceModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public CardBalanceModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<EditBalance> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<EditBalance> logger, ApplicationDbContext db, ICurrentUser currentUser)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    if (!_current_user.IsAdmin()) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_ACCESS_DENIED));

                    var model = request.Model;

                    var card = _db.Cards.FirstOrDefault(x => x.Number == model.Number && !x.IsDeleted);
                    if (card == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                    card.SetBalance(model.Balance);
                    await _db.SaveChangesAsync(cancellationToken);

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
