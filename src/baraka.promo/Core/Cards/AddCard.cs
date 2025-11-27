using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using baraka.promo.Models.Cards;
using baraka.promo.Models.LoyaltyApiModels;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.Cards
{
    public class AddCard
    {

        public class Command : IRequest<ApiBaseResultModel<LoyaltyResultModel>>
        {
            public Command(CardModel model, string integrationName)
            {
                IntegrationName = integrationName;
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public string IntegrationName { get; }
            public CardModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyResultModel>>
        {
            readonly ILogger<AddCard> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<AddCard> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    if (user != null && !_current_user.IsAdmin()) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    else if (!string.IsNullOrEmpty(request.IntegrationName)) user = request.IntegrationName;
                    else return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;

                    if (string.IsNullOrEmpty(model.Number))
                    {
                        var random = new Random();
                        model.Number = (DateTime.UtcNow.Ticks.ToString() + random.Next(1000, 9999)).Substring(0, 16);
                    }

                    if (!model.Number.All(char.IsNumber)) return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INVALID_NUMBER));

                    Guid card_id;

                    if(await _db.Cards.AnyAsync(x=>x.UserId == model.UserId && x.Type == model.Type && !x.IsDeleted)) 
                        card_id = await _db.Cards.Where(x => x.Type == model.Type && x.UserId == model.UserId && !x.IsDeleted).Select(z=> z.Id).FirstOrDefaultAsync();

                    else if (!_db.Cards.Any(x => x.Number == model.Number))
                    {
                        Card card = new Card(model.Number, model.UserId, 0, model.Type, model.HolderName, model.ProductInfo, user);

                        if (await _db.Cards.AnyAsync(cancellationToken))
                        {
                            card.HolderId = await _db.Cards.MaxAsync(x => x.HolderId) + 1;
                        }
                        else card.HolderId = 1;

                        await _db.Cards.AddAsync(card, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        card_id = card.Id;
                    }

                    //else card_id = _db.Cards.FirstOrDefault(x => x.Number == model.Number && x.UserId == model.UserId).Id;

                    else
                    {
                        return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INVALID_NUMBER));
                    }

                    return new ApiBaseResultModel<LoyaltyResultModel>(new LoyaltyResultModel { Id = card_id });
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
