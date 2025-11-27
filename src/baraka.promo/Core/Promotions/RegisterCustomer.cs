using baraka.promo.Data;
using baraka.promo.Delivery;
using baraka.promo.Models;
using baraka.promo.Models.PromoApi;
using baraka.promo.Services;
using baraka.promo.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using baraka.promo.Models.Enums;

namespace baraka.promo.Core.Promotions
{
    public class RegisterCustomer
    {
        public class Command : IRequest<ApiBaseResultModel>
        {
            public Command(CustomerApiModel model, string integrationName)
            {
                Model = model;
                IntegrationName = integrationName;
            }

            public CustomerApiModel Model { get; private set; }
            public string IntegrationName { get; set; }
        }

        public class Handler : IRequestHandler<Command, ApiBaseResultModel>
        {
            readonly ILogger<RegisterCustomer> _logger;
            readonly DeliveryDbContext _db;

            public Handler(ILogger<RegisterCustomer> logger, DeliveryDbContext db)
            {
                _logger = logger;
                _db = db;
            }

            public async Task<ApiBaseResultModel> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    var model = request.Model;
                    _logger.LogWarning($"RegisterCustomer -> {JsonConvert.SerializeObject(model)}");

                    if(await _db.Customers.AnyAsync(x=>x.Phone1 == model.Phone)) return new ApiBaseResultModel();

                    Customer customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Phone1 = model.Phone,
                        CreatedBy = request.IntegrationName,
                        CreatedTime = DateTime.Now,
                        ModifiedBy = request.IntegrationName,
                        ModifiedTime = DateTime.Now,
                        Language = Language.Ru,
                    };

                    await _db.Customers.AddAsync(customer, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    _logger.LogWarning($"RegisterCustomer -> send");

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
