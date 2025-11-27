using baraka.promo.Models;
using MediatR;
using baraka.promo.Models.LoyaltyApiModels.Cards;
using baraka.promo.Data.Loyalty;
using baraka.promo.Data;
using baraka.promo.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace baraka.promo.Core.Cards
{
    public class GetCard
    {
        public class Command : IRequest<ApiBaseResultModel<CardInfoModel>>
        {
            public Command(string card_id, string card_number)
            {
                CardId = card_id;
                CardNumber = card_number;
            }

            public string CardId { get; set; }
            public string CardNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<CardInfoModel>>
        {
            readonly ILogger<GetCard> _logger;
            readonly ApplicationDbContext _db;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GetCard> logger, ApplicationDbContext db, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<CardInfoModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {                   
                    var cache_key = CardInfoModel.CACHE_KEY + request.CardId + request.CardNumber;

                    if (!_memory_cache.TryGetValue(cache_key, out CardInfoModel result))
                    {
                        Card card = null;

                        if (!string.IsNullOrEmpty(request.CardId))
                        {
                            Guid card_id = Guid.Parse(request.CardId);
                            card = await _db.Cards.FirstOrDefaultAsync(x => x.Id == card_id && !x.IsDeleted);
                        }
                        else if (!string.IsNullOrEmpty(request.CardNumber))
                        {
                            card = await _db.Cards.FirstOrDefaultAsync(x => x.Number == request.CardNumber && !x.IsDeleted);
                        }
                        if(card == null)
                        {
                            var card_id = await _db.CardTokens.Where(t => t.Token == request.CardNumber && t.ExpiresAt >= DateTime.Now).Select(x=> x.CardId).FirstOrDefaultAsync(cancellationToken);
                            card = await _db.Cards.FirstOrDefaultAsync(x => x.Id == card_id && !x.IsDeleted);
                        }

                        if (card == null) return new ApiBaseResultModel<CardInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                        result = new CardInfoModel
                        {
                            Id = card.Id,
                            UserId = card.UserId,
                            Number = card.Number,
                            Balance = card.Balance,
                            Type = card.Type,
                            HolderId = card.HolderId,
                            HolderName = card.HolderName,
                            ProductInfo = card.ProductInfo,
                        };

                        _memory_cache.Set(cache_key, result, DateTimeOffset.Now.AddSeconds(5));
                    }

                    return new ApiBaseResultModel<CardInfoModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<CardInfoModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
