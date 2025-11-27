using baraka.promo.Data.Subscriptions;
using baraka.promo.Models.PaymeModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace baraka.promo.Services;

public class PaymeService
{
    readonly PaymeSettingsModel _settings;
    readonly ILogger<PaymeService> _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public PaymeService(IOptions<PaymeSettingsModel> options, ILogger<PaymeService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settings = options != null ? options.Value : new PaymeSettingsModel();
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    HttpClient GetClient()
    {
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.Add("X-Auth", $"{_settings.MerchantId}:{_settings.PaymeKey}");
        return http;
    }

    public async Task<string> ReceiptCreate(Subscription subscription, Guid transaction_id)
    {
        try
        {
            var model = new PaymeReceiptCreate
            {
                id = DateTime.Now.Ticks.ToString()
            };

            model.Params.Amount = subscription.Price * 100;
            model.Params.Account.OrderId = transaction_id;

            model.Params.Detail.Items = new List<PaymeReceiptCreate.ParamModel.DetailModel.ItemModel>
                {
                    new PaymeReceiptCreate.ParamModel.DetailModel.ItemModel
                    {
                        Title = subscription.NameRu,
                        Price = subscription.Price * 100,
                        Count = 1,
                        Code = subscription.MXIK,
                        VatPercent = subscription.Vat,
                        PackageCode = subscription.PackageCode,
                        //Units = subscription.UnitCode,
                    }
                };

            var http = GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"PaymeService: ReceiptCreate start -> {json}");
            var result = await http.PostAsync($"{_settings.Baseurl}/api", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = result.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"PaymeService: ReceiptCreate result -> {dataStr}");

            if (result.IsSuccessStatusCode)
            {
                var data_result = JsonConvert.DeserializeObject<PaymeReceiptResult>(dataStr);

                if (data_result.Error != null)
                {
                    _logger.LogError($"PaymeService: ReceiptCreate error -> {result.StatusCode} -> {data_result.Error.Message}");
                    return null;
                }

                else return data_result.Result.Receipt.Id;
            }
            else
            {
                _logger.LogError($"PaymeService: ReceiptCreate error -> {result.StatusCode} -> {dataStr}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<bool> ReceiptSend(string id, string phone)
    {
        try
        {
            var model = new PaymeReceiptSend
            {
                id = DateTime.Now.Ticks.ToString()
            };
            model.Params.Id = id;
            model.Params.Phone = phone;

            var http = GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"PaymeService: ReceiptSend start -> {json}");
            var response = await http.PostAsync($"{_settings.Baseurl}/api", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"PaymeService: ReceiptSend result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                var data_result = JsonConvert.DeserializeObject<PaymeReceiptSendResult>(dataStr);

                if (data_result.Result != null && data_result.Result.Success)
                {
                    return true;
                }
                else if (data_result.Error != null)
                {
                    _logger.LogError($"PaymeService: ReceiptSend error -> {response.StatusCode} -> {data_result.Error.Message}");
                    return false;
                }
                else return false;
            }
            else
            {
                _logger.LogError($"PaymeService: ReceiptSend error -> {response.StatusCode} -> {dataStr}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }

    public async Task<PaymeReceiptCheckResult> ReceiptCheck(string id)
    {
        try
        {
            var model = new PaymeReceiptCheck
            {
                id = DateTime.Now.Ticks.ToString(),
                Params = new PaymeReceiptCheck.ParamModel
                {
                    Id = id,
                }
            };

            var http = GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"PaymeService: ReceiptCheck start -> {json}");
            var response = await http.PostAsync($"{_settings.Baseurl}/api", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"PaymeService: ReceiptCheck result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<PaymeReceiptCheckResult>(dataStr);
            }
            else
            {
                _logger.LogError($"PaymeService: ReceiptSend error -> {response.StatusCode} -> {dataStr}");
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