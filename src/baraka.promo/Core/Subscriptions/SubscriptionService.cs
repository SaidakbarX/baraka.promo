using baraka.promo.Data;
using baraka.promo.Data.Subscriptions;
using baraka.promo.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace baraka.promo.Services
{
    public class SubscriptionService
    {
        private readonly ApplicationDbContext _db;
        private readonly DeliveryDbContext _deliveryDb;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SubscriptionService> _logger;
        private const string CACHE_KEY = "SubscriptionsListKey";

        public SubscriptionService(
            ApplicationDbContext db,
            DeliveryDbContext deliveryDb,
            IMemoryCache cache,
            ILogger<SubscriptionService> logger)
        {
            _db = db;
            _deliveryDb = deliveryDb;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<SubscriptionViewModel>> GetAllSubscriptions(int? countryId = null)
        {
            try
            {
                var query = _db.Subscriptions.Where(s => !s.IsActive);

                if (countryId.HasValue)
                {
                    query = query.Where(s => s.CountryId == countryId.Value);
                }

                var subscriptions = await query
                    .OrderByDescending(s => s.CreatedTime)
                    .Select(s => new SubscriptionViewModel
                    {
                        Id = s.Id,
                        NameUz = s.NameUz,
                        NameRu = s.NameRu,
                        NameEn = s.NameEn,
                        ProductId = s.ProductId,
                        Price = s.Price,
                        MaxCount = s.MaxCount,
                        IsActive = s.IsActive,
                        CountryId = s.CountryId,
                        GroupId = s.GroupId,
                        CreatedTime = s.CreatedTime,
                        ShortContent_Uz = s.ShortContent_Uz,
                        ShortContent_Ru = s.ShortContent_Ru,
                        ShortContent_En = s.ShortContent_En
                    })
                    .ToListAsync();

                foreach (var sub in subscriptions)
                {
                    sub.ClientCount = await _db.SubscriptionClients
                        .Where(sc => sc.SubscriptionId == sub.Id)
                        .CountAsync();
                }

                return subscriptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions");
                return new List<SubscriptionViewModel>();
            }
        }

        public async Task<Subscription?> GetSubscriptionById(long id)
        {
            try
            {
                return await _db.Subscriptions
                    .FirstOrDefaultAsync(s => s.Id == id && !s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscription by id: {id}");
                return null;
            }
        }

        public async Task<(bool success, string? error, long? id)> CreateSubscription(Subscription model, string? userName)
        {
            try
            {
                model.CreatedTime = DateTime.Now;
                model.IsActive = false;

                await _db.Subscriptions.AddAsync(model);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Subscription created by {userName}, ID: {model.Id}");

                // Cache'ni tozalash
                _cache.Remove(CACHE_KEY);

                return (true, null, model.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return (false, ex.Message, null);
            }
        }

        // Subscriptionni yangilash
        public async Task<(bool success, string? error)> UpdateSubscription(Subscription model, string? userName)
        {
            try
            {
                var existing = await _db.Subscriptions.FindAsync(model.Id);

                if (existing == null)
                {
                    return (false, "Subscription topilmadi");
                }

                // Faqat o'zgartirilishi mumkin bo'lgan fieldlarni yangilash
                existing.NameUz = model.NameUz;
                existing.NameRu = model.NameRu;
                existing.NameEn = model.NameEn;
                existing.ProductId = model.ProductId;
                existing.Price = model.Price;
                existing.MaxCount = model.MaxCount;
                existing.IsActive = model.IsActive;
                existing.CountryId = model.CountryId;
                existing.GroupId = model.GroupId;
                existing.ShortContent_Uz = model.ShortContent_Uz;
                existing.ShortContent_Ru = model.ShortContent_Ru;
                existing.ShortContent_En = model.ShortContent_En;
                existing.Content_Uz = model.Content_Uz;
                existing.Content_Ru = model.Content_Ru;
                existing.Content_En = model.Content_En;
                existing.ImageUrlUz = model.ImageUrlUz;
                existing.ImageUrlRu = model.ImageUrlRu;
                existing.ImageUrlEn = model.ImageUrlEn;
                existing.NamePurchaseUz = model.NamePurchaseUz;
                existing.NamePurchaseRu = model.NamePurchaseRu;
                existing.NamePurchaseEn = model.NamePurchaseEn;
                existing.ShortContent_Purchase_Uz = model.ShortContent_Purchase_Uz;
                existing.ShortContent_Purchase_Ru = model.ShortContent_Purchase_Ru;
                existing.ShortContent_Purchase_En = model.ShortContent_Purchase_En;
                existing.Content_Purchase_Uz = model.Content_Purchase_Uz;
                existing.Content_Purchase_Ru = model.Content_Purchase_Ru;
                existing.Content_Purchase_En = model.Content_Purchase_En;
                existing.ImageUrl_Purchase_Uz = model.ImageUrl_Purchase_Uz;
                existing.ImageUrl_Purchase_Ru = model.ImageUrl_Purchase_Ru;
                existing.ImageUrl_Purchase_En = model.ImageUrl_Purchase_En;
                existing.PromotionId = model.PromotionId;
                existing.MXIK = model.MXIK;
                existing.PackageCode = model.PackageCode;
                existing.Vat = model.Vat;
                existing.UnitCode = model.UnitCode;

                await _db.SaveChangesAsync();

                _logger.LogInformation($"Subscription updated by {userName}, ID: {model.Id}");

                // Cache'ni tozalash
                _cache.Remove(CACHE_KEY);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription");
                return (false, ex.Message);
            }
        }

        // Subscriptionni o'chirish (soft delete)
        public async Task<(bool success, string? error)> DeleteSubscription(long id, string? userName)
        {
            try
            {
                var subscription = await _db.Subscriptions.FindAsync(id);

                if (subscription == null)
                {
                    return (false, "Subscription topilmadi");
                }

                subscription.IsActive = true;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"Subscription deleted by {userName}, ID: {id}");

                // Cache'ni tozalash
                _cache.Remove(CACHE_KEY);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription");
                return (false, ex.Message);
            }
        }

        // Subscription grouplarni olish
        public async Task<List<SubscriptionGroupViewModel>> GetSubscriptionGroups(int? countryId = null)
        {
            try
            {
                var query = _db.SubscriptionGroups.Where(sg => sg.IsActive);

                if (countryId.HasValue)
                {
                    query = query.Where(sg => sg.CountryId == countryId.Value);
                }

                return await query
                    .OrderBy(sg => sg.NameUz)
                    .Select(sg => new SubscriptionGroupViewModel
                    {
                        Id = sg.Id,
                        NameUz = sg.NameUz,
                        NameRu = sg.NameRu,
                        NameEn = sg.NameEn,
                        CountryId = sg.CountryId,
                        ValidityPeriod = sg.ValidityPeriod
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription groups");
                return new List<SubscriptionGroupViewModel>();
            }
        }

        // Subscription clientlarni olish
        public async Task<List<SubscriptionClientViewModel>> GetSubscriptionClients(long subscriptionId)
        {
            try
            {
                var clients = await (from sc in _db.SubscriptionClients
                                     join c in _deliveryDb.Customers on sc.Id equals c.Id
                                     where sc.SubscriptionId == subscriptionId
                                     select new SubscriptionClientViewModel
                                     {
                                         Id = sc.Id,
                                         SubscriptionId = sc.SubscriptionId,
                                         TimeOfUse = sc.TimeOfUse,
                                         CustomerName = c.FirstName + " " + c.LastName,
                                         Phone = c.Phone1
                                     })
                                    .ToListAsync();

                return clients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting subscription clients for subscription: {subscriptionId}");
                return new List<SubscriptionClientViewModel>();
            }
        }
    }

    // View Models
    public class SubscriptionViewModel
    {
        public long Id { get; set; }
        public string NameUz { get; set; }
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public int MaxCount { get; set; }
        public bool IsActive { get; set; }
        public int CountryId { get; set; }
        public long GroupId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? ShortContent_Uz { get; set; }
        public string? ShortContent_Ru { get; set; }
        public string? ShortContent_En { get; set; }
        public int ClientCount { get; set; }
    }

    public class SubscriptionGroupViewModel
    {
        public long Id { get; set; }
        public string NameUz { get; set; }
        public string? NameRu { get; set; }
        public string? NameEn { get; set; }
        public int CountryId { get; set; }
        public DateTime? ValidityPeriod { get; set; }
    }

    public class SubscriptionClientViewModel
    {
        public Guid Id { get; set; }
        public long SubscriptionId { get; set; }
        public DateTime? TimeOfUse { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
    }
}
