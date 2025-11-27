using baraka.promo.Core.Cardholders;
using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;

namespace baraka.promo.Core.Transactions
{
    public class DeleteTransaction
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<DeleteTransaction> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<DeleteTransaction> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    var transaction = _db.Transactions.Find(request.Id);

                    if (transaction == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_TRANSACTION_NOT_FOUND));

                    transaction.SetStatus(Models.Enums.TransactionStatus.Cancelled);
                    transaction.SetCanceledTime(DateTime.Now);

                    await _db.SaveChangesAsync(cancellationToken);

                    var card = _db.Cards.FirstOrDefault(x => x.Id == transaction.CardId);

                    if(transaction.Type == Models.Enums.TransactionType.WriteOff) card.PlusBalance(transaction.Sum);
                    else if (transaction.Type == Models.Enums.TransactionType.Replenishment) card.MinusBalance(transaction.Sum);

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
