using baraka.promo.Models.Cards;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;

namespace baraka.promo.Core.Cards
{
    public class CheckNumber
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(string number)
            {
                Number = number;
            }

            public string Number { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<CheckNumber> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<CheckNumber> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    if(_db.Cards.Any(x=>x.Number == request.Number && !x.IsDeleted)) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_DUPLICATE_CARD));

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
