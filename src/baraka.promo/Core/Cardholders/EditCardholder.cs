using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;

namespace baraka.promo.Core.Cardholders
{
    public class EditCardholder
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(Guid id, CardholderModel model)
            {
                Id = id;
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public Guid Id { get; set; }
            public CardholderModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<EditCardholder> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<EditCardholder> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    if(cardholder == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_USER_NOT_FOUND));

                    var model = request.Model;

                    cardholder.Update(model.Name, model.Phone, model.DateOfBirth, model.Type, user,model.Email,model.Sex);

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
