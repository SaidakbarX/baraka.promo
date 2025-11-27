using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;

namespace baraka.promo.Core.Cardholders
{
    public class AddCardholder
    {
        public class Command : IRequest<ApiBaseResultModel<LoyaltyResultModel>>
        {
            public Command(CardholderModel model)
            {
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public CardholderModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyResultModel>>
        {
            readonly ILogger<AddCardholder> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<AddCardholder> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    Cardholder cardholder = new Cardholder(model.Name, model.Phone, model.DateOfBirth, model.Type, user,model.Email,model.Sex);

                    await _db.Cardholders.AddAsync(cardholder, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    return new ApiBaseResultModel<LoyaltyResultModel>(new LoyaltyResultModel { Id = cardholder.Id });
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
