using baraka.promo.Data;
using baraka.promo.Data.Loyalty;
using baraka.promo.Models.Enums;
using baraka.promo.Models.LoyaltyApiModels.LoyalityTypeModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace baraka.promo.Services.Loyality
{
    public class LoyalitySettingService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<LoyalitySettingService> _logger;
        private readonly ICurrentUser _currentUser;

        public LoyalitySettingService(
            ApplicationDbContext db,
            ILogger<LoyalitySettingService> logger,
            ICurrentUser currentUser)
        {
            _db = db;
            _logger = logger;
            _currentUser = currentUser;
        }
        public async Task SetInactive(long id)
        {
            var setting = await _db.LoyalityTypes.FindAsync(id);
            if (setting != null)
            {
                setting.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }
        public async Task<List<LoyalityType>> GetAllByType(LoyalityTypeKey typeKey)
        {
            try
            {
                return await _db.LoyalityTypes
                    .Where(x => x.Type == typeKey.ToString())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loyality types by key: {TypeKey}", typeKey);
                return new List<LoyalityType>();
            }
        }

        public async Task<LoyalityType?> GetFirstByType(LoyalityTypeKey typeKey)
        {
            try
            {
                return await _db.LoyalityTypes
                    .FirstOrDefaultAsync(x => x.Type == typeKey.ToString() && x.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting first loyality type by key: {TypeKey}", typeKey);
                return null;
            }
        }

        public async Task<LoyalityType?> GetById(int id)
        {
            try
            {
                return await _db.LoyalityTypes.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loyality type by id: {Id}", id);
                return null;
            }
        }

        public async Task<LoyalityType?> Create(LoyalityTypeKey typeKey, string valueInfo, bool isActive = true)
        {
            try
            {
                var newSetting = new LoyalityType
                {
                    Type = typeKey.ToString(),
                    ValueInfo = valueInfo,
                    IsActive = isActive
                };

                await _db.LoyalityTypes.AddAsync(newSetting);
                await _db.SaveChangesAsync();
                return newSetting;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating loyality type");
                return null;
            }
        }

        public async Task<bool> UpdateValueInfo(int id, string valueInfo)
        {
            try
            {
                var setting = await _db.LoyalityTypes.FindAsync(id);
                if (setting == null) return false;

                setting.ValueInfo = valueInfo;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating loyality type value info: {Id}", id);
                return false;
            }
        }

        public async Task<bool> SetActive(int id, bool isActive)
        {
            try
            {
                var setting = await _db.LoyalityTypes.FindAsync(id);
                if (setting == null) return false;

                setting.IsActive = isActive;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting loyality type active status: {Id}", id);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var setting = await _db.LoyalityTypes.FindAsync(id);
                if (setting == null) return false;

                _db.LoyalityTypes.Remove(setting);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting loyality type: {Id}", id);
                return false;
            }
        }

        public async Task<LoyalityType?> GetCashbackSettings()
        {
            try
            {
                return await _db.LoyalityTypes
                    .FirstOrDefaultAsync(x =>
                        x.Type == LoyalityTypeKey.CASHBACK.ToString() &&
                        x.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cashback settings");
                throw;
            }
        }

        public async Task<bool> UpdateCashbackSettings(string valueInfo)
        {
            try
            {
                var cashbackSettings = await _db.LoyalityTypes
                    .FirstOrDefaultAsync(x =>
                        x.Type == LoyalityTypeKey.CASHBACK.ToString() &&
                        x.IsActive);

                if (cashbackSettings != null)
                {
                    cashbackSettings.ValueInfo = valueInfo;
                    _db.LoyalityTypes.Update(cashbackSettings);
                }
                else
                {
                    var newSettings = new LoyalityType
                    {
                        Type = LoyalityTypeKey.CASHBACK.ToString(),
                        ValueInfo = valueInfo,
                        IsActive = true,
                    };
                    await _db.LoyalityTypes.AddAsync(newSettings);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cashback settings");
                return false;
            }
        }

        public async Task<bool> CreateDefaultCashbackSettings()
        {
            try
            {
                var exists = await _db.LoyalityTypes
                    .AnyAsync(x =>
                        x.Type == LoyalityTypeKey.CASHBACK.ToString() &&
                        x.IsActive);

                if (!exists)
                {
                    var defaultCashbacks = new List<CashbackModel>
                    {
                        new CashbackModel
                        {
                            CardType = CardType.Common,
                            Value = 3,
                            ImgUrlBlack = "",
                            ImgUrlLight = "",
                            Oferta = new LanguageInfo
                            {
                                OfertaUz = "Umumiy karta uchun oferta",
                                OfertaRu = "Оферта для общей карты",
                                OfertaEn = "Offer for common card"
                            }
                        },
                        new CashbackModel
                        {
                            CardType = CardType.Silver,
                            Value = 5,
                            ImgUrlBlack = "",
                            ImgUrlLight = "",
                            Oferta = new LanguageInfo
                            {
                                OfertaUz = "Kumush karta uchun oferta",
                                OfertaRu = "Оферта для серебряной карты",
                                OfertaEn = "Offer for silver card"
                            }
                        },
                        new CashbackModel
                        {
                            CardType = CardType.Gold,
                            Value = 7,
                            ImgUrlBlack = "",
                            ImgUrlLight = "",
                            Oferta = new LanguageInfo
                            {
                                OfertaUz = "Oltin karta uchun oferta",
                                OfertaRu = "Оферта для золотой карты",
                                OfertaEn = "Offer for gold card"
                            }
                        }
                    };

                    var defaultSettings = new LoyalityType
                    {
                        Type = LoyalityTypeKey.CASHBACK.ToString(),
                        ValueInfo = JsonConvert.SerializeObject(defaultCashbacks),
                        IsActive = true,
                    };

                    await _db.LoyalityTypes.AddAsync(defaultSettings);
                    await _db.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating default cashback settings");
                return false;
            }
        }

        public async Task<bool> ActivateCashbackSettings(int id)
        {
            try
            {
                var allCashbackSettings = await _db.LoyalityTypes
                    .Where(x => x.Type == LoyalityTypeKey.CASHBACK.ToString())
                    .ToListAsync();

                foreach (var setting in allCashbackSettings)
                {
                    setting.IsActive = setting.Id == id;
                }

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating cashback settings");
                return false;
            }
        }

        public async Task<List<CashbackModel>> GetCashbackModelList()
        {
            try
            {
                var settings = await GetCashbackSettings();

                if (settings != null && !string.IsNullOrEmpty(settings.ValueInfo))
                {
                    return JsonConvert.DeserializeObject<List<CashbackModel>>(settings.ValueInfo) ?? new List<CashbackModel>();
                }

                return new List<CashbackModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cashback model list");
                return new List<CashbackModel>();
            }
        }

        public async Task<CashbackModel?> GetCashbackByCardType(CardType cardType)
        {
            try
            {
                var cashbacks = await GetCashbackModelList();
                return cashbacks.FirstOrDefault(x => x.CardType == cardType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cashback by card type: {CardType}", cardType);
                return null;
            }
        }
    }
}