using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;

namespace baraka.promo.Core.Cardholders
{
    public class DeleteCardholder
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
            readonly ILogger<DeleteCardholder> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<DeleteCardholder> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    var cardholder = _db.Cardholders.Find(request.Id);

                    if (cardholder == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_USER_NOT_FOUND));

                    cardholder.Delete(user);

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
