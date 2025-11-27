using baraka.promo.Models.LoyaltyApiModels.Cardholders;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Models;
using MediatR;
using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Services;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;
using baraka.promo.Models.LoyaltyApiModels.Cards;

namespace baraka.promo.Core.Cardholders
{
    public class GetCardholder
    {
        public class Command : IRequest<ApiBaseResultModel<CardholderInfoModel>>
        {
            public Command(string cardholder_id, string cardholder_phone, string card_id, string card_number)
            {
                CardholderId = cardholder_id;
                CardholderPhone = cardholder_phone;
                CardId = card_id;
                CardNumber = card_number;
            }

            public string CardholderId { get; set; }
            public string CardholderPhone { get; set; }
            public string CardId { get; set; }
            public string CardNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<CardholderInfoModel>>
        {
            readonly ILogger<GetCardholder> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetCardholder> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<CardholderInfoModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _current_user.GetCurrentUserName();
                    if (user == null) return new ApiBaseResultModel<CardholderInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_UNAUTHORIZED));

                    var cache_key = CardholderInfoModel.CACHE_KEY + request.CardholderId + request.CardholderPhone + request.CardId + request.CardNumber;

                    if (!_memory_cache.TryGetValue(cache_key, out CardholderInfoModel result))
                    {
                        Cardholder cardholder = null;
                        Card card = null;

                        if(!string.IsNullOrEmpty(request.CardholderId))
                        {
                            Guid cardholder_id = Guid.Parse(request.CardholderId);
                            cardholder = _db.Cardholders.FirstOrDefault(x => x.Id == cardholder_id && !x.IsDeleted);
                        }
                        else if (!string.IsNullOrEmpty(request.CardholderPhone))
                        {
                            cardholder = _db.Cardholders.FirstOrDefault(x => x.Phone == request.CardholderPhone && !x.IsDeleted);
                        }

                        if (!string.IsNullOrEmpty(request.CardId))
                        {
                            Guid card_id = Guid.Parse(request.CardId);
                            card = _db.Cards.FirstOrDefault(x => x.Id == card_id && !x.IsDeleted);
                        }
                        else if(!string.IsNullOrEmpty(request.CardNumber))
                        {
                            card = _db.Cards.FirstOrDefault(x => x.Number == request.CardNumber && !x.IsDeleted);
                        }

                        if(cardholder == null)
                        {
                            if(card != null)
                            {
                                cardholder = _db.Cardholders.FirstOrDefault(x=>x.Id == card.UserId && !x.IsDeleted);
                            }
                            else return new ApiBaseResultModel<CardholderInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));
                        }

                        result = new CardholderInfoModel
                        {
                            Id = cardholder.Id,
                            Name = cardholder.Name,
                            Phone = cardholder.Phone,
                            Type = cardholder.Type,
                            DateOfBirth = cardholder.DateOfBirth?.ToShortDateString(),
                            Email = cardholder.Email,
                            Sex = cardholder.Sex
                        };

                        if (card == null)
                        {
                            var cards = _db.Cards.Where(x => x.UserId == cardholder.Id && !x.IsDeleted)
                                .Select(z => new CardInfoModel { Id = z.Id, UserId = z.UserId, Balance = z.Balance, Number = z.Number, Type = z.Type });

                            result.Cards = cards.ToList();
                        }
                        else result.Cards = new List<CardInfoModel> { new CardInfoModel { Id = card.Id, UserId = card.UserId, Balance = card.Balance, Number = card.Number, Type = card.Type } };

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(5));
                    }

                    return new ApiBaseResultModel<CardholderInfoModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<CardholderInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
