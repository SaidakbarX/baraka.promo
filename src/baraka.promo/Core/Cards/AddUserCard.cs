using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models;
using baraka.promo.Models.Cards;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace baraka.promo.Core.Cards
{
    public class AddUserCard
    {
        public class Command : IRequest<ApiBaseResultModel<LoyaltyCardResultModel>>
        {
            public Command(UserCardModel model, string integrationName)
            {
                IntegrationName = integrationName;
                Model = model ?? throw new ArgumentNullException(nameof(model));
            }

            public string IntegrationName { get; }
            public UserCardModel Model { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<LoyaltyCardResultModel>>
        {
            readonly ILogger<AddUserCard> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<AddUserCard> logger, ApplicationDbContext db, ICurrentUser currentUser)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
            }

            public async Task<ApiBaseResultModel<LoyaltyCardResultModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();

                    if (user != null && !_current_user.IsAdmin()) return new ApiBaseResultModel<LoyaltyCardResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));
                    else if (!string.IsNullOrEmpty(request.IntegrationName)) user = request.IntegrationName;
                    else return new ApiBaseResultModel<LoyaltyCardResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var model = request.Model;

                    _logger.LogWarning($"AddUserCard -> {JsonConvert.SerializeObject(model)}");

                    Guid card_id;
                    Guid user_id;

                    string phone = Regex.Replace(model.Phone, @"[+()\s-]", "");

                    if (!await _db.Cardholders.AnyAsync(x => x.Phone == phone && !x.IsDeleted))
                    {
                        Cardholder cardholder = new Cardholder(model.FullName, phone, model.DateOfBirth, Models.Enums.CardholderType.Common, user,model.Email,model.Sex);
                       

                        await _db.Cardholders.AddAsync(cardholder, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        user_id = cardholder.Id;
                    }
                    else user_id = await _db.Cardholders.Where(x => x.Phone == phone && !x.IsDeleted).Select(x => x.Id).FirstOrDefaultAsync();

                    if (await _db.Cards.AnyAsync(x => x.UserId == user_id && !x.IsDeleted))
                        card_id = await _db.Cards.Where(x => x.UserId == user_id && !x.IsDeleted).Select(z => z.Id).FirstOrDefaultAsync();

                    else
                    {
                        long num = 0;

                        if (await _db.Cards.AnyAsync(cancellationToken))
                        {
                            num = await _db.Cards.MaxAsync(x => x.HolderId) + 1;
                        }
                        else num = 1;

                        var random = new Random();
                        string number = (DateTime.UtcNow.Ticks.ToString() + random.Next(1000, 9999)).Substring(0, 15);

                        number += num;
                        Card card = new Card(number, user_id, 0, Models.Enums.CardType.Common, model.FullName, null, user);
                        card.HolderId = num;

                        await _db.Cards.AddAsync(card, cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);

                        card_id = card.Id;
                    }

                    //else
                    //{
                    //    return new ApiBaseResultModel<LoyaltyResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INVALID_NUMBER));
                    //}

                    var result = new ApiBaseResultModel<LoyaltyCardResultModel>(new LoyaltyCardResultModel { UserId = user_id });

                    _logger.LogWarning($"AddUserCard send -> {JsonConvert.SerializeObject(result)}");

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<LoyaltyCardResultModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
