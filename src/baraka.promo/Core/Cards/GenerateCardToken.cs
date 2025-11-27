using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models;
using baraka.promo.Models.LoyaltyApiModels;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace baraka.promo.Core.Cards
{
    public class GenerateCardToken
    {
        public class Command : IRequest<ApiBaseResultModel<TokenModel>>
        {
            public Command(Guid card_id)
            {
                CardId = card_id;
            }

            public Guid CardId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel<TokenModel>>
        {
            readonly ILogger<GenerateCardToken> _logger;
            readonly ApplicationDbContext _db;
            readonly IMemoryCache _memory_cache;

            public Handler(ILogger<GenerateCardToken> logger, ApplicationDbContext db, IMemoryCache memoryCache)
            {
                _logger = logger;
                _db = db;
                _memory_cache = memoryCache;
            }

            public async Task<ApiBaseResultModel<TokenModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Guid card_id = request.CardId;
                    _logger.LogWarning($"GenerateCardToken -> {card_id}");

                    string cache_key = TextConstants.CardTokenInfoCacheKey + card_id;

                    if (!_memory_cache.TryGetValue(cache_key, out TokenModel result))
                    {
                        bool cardExists = await _db.Cards.AnyAsync(c => c.Id == card_id && !c.IsDeleted);
                        if (!cardExists) return new ApiBaseResultModel<TokenModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_CARD_NOT_FOUND));

                        DateTime now = DateTime.Now;


                        // Удаляем просроченные токены по ReservedUntil (для других карт тоже),
                        //    чтобы номер мог повторно использоваться.

                        if(await _db.CardTokens.AnyAsync(x => x.ReservedUntil < now))
                        {
                            var expiredTokens = _db.CardTokens.Where(x => x.ReservedUntil < now);
                            _db.CardTokens.RemoveRange(expiredTokens);
                            await _db.SaveChangesAsync(cancellationToken);
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            var bytes = new byte[4];
                            RandomNumberGenerator.Fill(bytes);
                            uint v = BitConverter.ToUInt32(bytes, 0);
                            long num = v % 1_000_000_000L;
                            var token = num.ToString("D9");

                            if (!await _db.CardTokens.AnyAsync(_ => _.Token == token && _.ReservedUntil > now))
                            {
                                now = DateTime.Now;
                                DateTime expire_time = now.AddMinutes(3);
                                CardToken entity = new CardToken(card_id, token, now, expire_time, now.AddHours(5));

                                await _db.CardTokens.AddAsync(entity, cancellationToken);
                                await _db.SaveChangesAsync(cancellationToken);

                                result = new TokenModel { Token = token, Expires = expire_time };

                                break;
                            }
                        }

                        if(result != null) _memory_cache.Set(cache_key, result, DateTime.Now.AddMinutes(2));
                        else result = new TokenModel();
                    }

                    _logger.LogWarning($"GenerateCardToken -> {card_id} send");

                    return new ApiBaseResultModel<TokenModel>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return new ApiBaseResultModel<TokenModel>(ErrorHepler.GetError(ErrorHeplerType.ERROR_INTERNAL));
                }
            }
        }
    }
}
