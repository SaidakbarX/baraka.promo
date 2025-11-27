using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models;
using baraka.promo.Models.Cards;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace baraka.promo.Core.Cards
{
    public class GetUserCard
    {
        public class Command : IRequest<ApiBaseResultModel<UserCardInfoModel>>
        {
            public Command(string cardholder_id)
            {
                CardholderId = cardholder_id;
                //CardholderPhone = cardholder_phone;
                //CardId = card_id;
                //CardNumber = card_number;
            }

            public string CardholderId { get; set; }
            //public string CardholderPhone { get; set; }
            //public string CardId { get; set; }
            //public string CardNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<UserCardInfoModel>>
        {
            readonly ILogger<GetUserCard> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetUserCard> logger, ApplicationDbContext db, ICurrentUser currentUser, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _current_user = currentUser;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<UserCardInfoModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    //_logger.LogWarning($"GetUserCard -> {card_id}");

                    var cache_key = UserCardInfoModel.CACHE_KEY + request.CardholderId;

                    if (!_memory_cache.TryGetValue(cache_key, out UserCardInfoModel result))
                    {
                        Cardholder cardholder = null;
                        //Card card = null;

                        if (!string.IsNullOrEmpty(request.CardholderId))
                        {
                            Guid cardholder_id = Guid.Parse(request.CardholderId);
                            cardholder = await _db.Cardholders.FirstOrDefaultAsync(x => x.Id == cardholder_id && !x.IsDeleted);
                        }
                        //else if (!string.IsNullOrEmpty(request.CardholderPhone))
                        //{
                        //    string phone = Regex.Replace(request.CardholderPhone, @"[+()\s-]", "");
                        //    cardholder = await _db.Cardholders.FirstOrDefaultAsync(x => x.Phone == phone && !x.IsDeleted);
                        //}

                        if (cardholder == null) return new ApiBaseResultModel<UserCardInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                        result = new UserCardInfoModel
                        {
                            User = new UserCardModel
                            {
                                FullName = cardholder.Name,
                                Phone = cardholder.Phone,
                                Sex = cardholder.Sex,
                                DateOfBirth = cardholder.DateOfBirth,
                                Email = cardholder.Email,
                            }
                        };

                        var cards = _db.Cards.Where(x => x.UserId == cardholder.Id && !x.IsDeleted)
                                .Select(z => new CardDetailsModel
                                {
                                    Id = z.Id,
                                    HolderName = z.HolderName,
                                    Balance = z.Balance,
                                    Type = z.Type.ToString(),
                                    //ImageUrl = 
                                });

                        result.Cards = cards.ToList();

                        _memory_cache.Set(cache_key, result, DateTime.Now.AddSeconds(5));
                    }

                    return new ApiBaseResultModel<UserCardInfoModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<UserCardInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
