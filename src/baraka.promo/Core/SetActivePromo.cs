using baraka.promo.Data;
using baraka.promo.Models;
using baraka.promo.Models.PromoApi;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace baraka.promo.Core
{
    public class SetActivePromo
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(long id)
            {
                Id = id;
            }

            public long Id { get; private set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<SetActivePromo> _logger;
            readonly ApplicationDbContext _db;
            readonly ICurrentUser _current_user;

            public Handler(ILogger<SetActivePromo> logger, ApplicationDbContext db, ICurrentUser currentUser)
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

                    long promo_id = request.Id;
                    _logger.LogWarning($"SetActivePromo -> {promo_id}");

                    var promo = await _db.Promos.FirstOrDefaultAsync(x => x.Id == promo_id, cancellationToken);

                    if (promo == null) return new ApiBaseResultModel(ErrorHepler.GetError(ErrorHeplerType.ERROR_NOT_FOUND));

                    promo.IsActive = true;
                    await _db.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning($"SetActivePromo -> {promo_id} send");

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
