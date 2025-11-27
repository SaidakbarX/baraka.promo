using baraka.promo.Data.Subscriptions;
using baraka.promo.Models.CardPaymentModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace baraka.promo.Services;

public class CardPaymentService
{


    readonly BarakaSettingsModel _baraka_settings;
    readonly ILogger<CardPaymentService> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    readonly IMemoryCache _memoryCache;
    readonly CardPaymentSettings _paymentSettings;
    readonly ICurrentUser _currentUser;

    string cacheKey(BarakaTokenModel model) => $"CardPaymentService.BarakaToken.ApiLogin={model.userName}";

    public CardPaymentService(IOptions<BarakaSettingsModel> options, ILogger<CardPaymentService> logger,
        IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IOptions<CardPaymentSettings> cardPaymentSettings,
        ICurrentUser currentUser)
    {
        _baraka_settings = options != null ? options.Value : new BarakaSettingsModel();
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _paymentSettings = cardPaymentSettings != null ? cardPaymentSettings.Value : new CardPaymentSettings();
        _currentUser = currentUser;
    }

    HttpClient GetClient()
    {
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //string token = GetToken();
        //http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return http;
    }

    HttpContent getContent<T>(T model)
    {
        var json = JsonConvert.SerializeObject(model);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    async Task<string> GetBarakaToken(bool from_cache = true)
    {
        BarakaTokenModel model = new()
        {
            userName = _baraka_settings.Username,
            password = _baraka_settings.Password,
        };

        if (!_memoryCache.TryGetValue(cacheKey(model), out string token) && from_cache)
        {
            var http = _httpClientFactory.CreateClient();
            http.Timeout = TimeSpan.FromSeconds(15);

            var result = await http.PostAsync($"{_baraka_settings.Url}/api/account/token", getContent(model));
            string dataStr = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<BarakaTokenResponse>(dataStr);
                token = data.token;
                _memoryCache.Set(cacheKey(model), token, DateTimeOffset.Now.AddMinutes(15));
            }
            else throw new Exception($"{result.StatusCode} -> {dataStr}");
        }
        return token;
    }

    async Task<string> GetToken(PaymentTokenModel model, bool from_cache = true)
    {
        var http = GetClient();
        //string token = await GetBarakaToken(from_cache);

        string token = _currentUser.GetCurrentToken();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var result = await http.PostAsync($"{_paymentSettings.Url}/token", getContent(model));
        string dataStr = await result.Content.ReadAsStringAsync();

        if (result.IsSuccessStatusCode)
        {
            var data = JsonConvert.DeserializeObject<BarakaTokenResponse>(dataStr);
            token = data.token;
        }
        //else if(result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //{
        //    await GetToken(model, false);
        //}
        else throw new Exception($"{result.StatusCode} -> {dataStr}");

        return token;
    }

    public async Task<string> CreatePayment(PaymentTokenModel token_model, Subscription subscription, Guid transaction_id)
    {
        try
        {
            var http = GetClient();
            string token = await GetToken(token_model);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            List<OfdModel> ofdModels = new List<OfdModel>
                {
                    new OfdModel
                    {
                            mxik = subscription.MXIK,
                            name = subscription.NameRu,
                            package_code = subscription.PackageCode,
                            price = subscription.Price,
                            total = subscription.Price,
                            qty = 1,
                            vat = subscription.Vat,
                            unit = subscription.UnitCode,
                            tin = _paymentSettings.Tin,
                    }
                };

            CardPaymentModel payment_model = new CardPaymentModel
            {
                order_id = transaction_id,
                amount = subscription.Price,
                store_id = _paymentSettings.StoreId,
                ofd = ofdModels,
            };

            var json = JsonConvert.SerializeObject(payment_model);

            _logger.LogWarning($"CardPaymentService: CreatePayment start -> {json}");
            var response = await http.PostAsync($"{_paymentSettings.Url}/payment", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"CardPaymentService: CreatePayment result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<CardPaymentResponse>(dataStr)?.id;
            }
            else
            {
                _logger.LogError($"CardPaymentService: CreatePayment error -> {response.StatusCode} -> {dataStr}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}

