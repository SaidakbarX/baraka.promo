using baraka.promo.Core.Cards;
using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;

namespace baraka.promo.Core.Cardholders
{
    public class CheckPhone
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(string phone)
            {
                Phone = phone;
            }

            public string Phone { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<CheckPhone> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<CheckPhone> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    if (_db.Cardholders.Any(x => x.Phone == request.Phone && !x.IsDeleted)) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_PHONE_EXISTS));

                    else return new ApiBaseResultModel();
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
