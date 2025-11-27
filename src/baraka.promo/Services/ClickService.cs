using baraka.promo.Data.Subscriptions;
using baraka.promo.Models.ClickModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace baraka.promo.Services;

public class ClickService
{
    readonly ClickSettingsModel _settings;
    readonly ILogger<ClickService> _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public ClickService(IOptions<ClickSettingsModel> options, ILogger<ClickService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settings = options != null ? options.Value : new ClickSettingsModel();
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    HttpClient GetClient()
    {
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.Add("Auth", GetAuthHeader());

        return http;
    }

    string GetAuthHeader()
    {
        var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var pass = $"{time}{_settings.SecretKey}";
        var hesh = Sha1(pass);
        return $"{_settings.MerchantUserId}:{hesh}:{time}";
    }
    string Sha1(string input)
    {
        StringBuilder hash = new StringBuilder();

        using (var sha1 = System.Security.Cryptography.SHA1.Create())
        {
            byte[] bytes = sha1.ComputeHash(new UTF8Encoding().GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
        }
        return hash.ToString();
    }

    public async Task<string> ReceiptCreate(Subscription subscription, Guid transaction_id, string phone)
    {
        try
        {
            var model = new ClickReceiptCreate
            {
                service_id = _settings.ServiceId,
                amount = subscription.Price,
                phone_number = phone,
                merchant_trans_id = transaction_id,
            };

            var http = GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"ClickService: ReceiptCreate start -> {json}");
            var response = await http.PostAsync($"{_settings.Baseurl}/invoice/create", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"ClickService: ReceiptCreate result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                var data_result = JsonConvert.DeserializeObject<ClickReceiptResult>(dataStr);

                if (data_result.error_code != 0 || !string.IsNullOrWhiteSpace(data_result.error_note))
                {
                    _logger.LogError($"PaymeService: ReceiptCreate error -> {response.StatusCode} -> {data_result.error_note} -> {data_result.error_code}");
                    return null;
                }

                else return data_result.invoice_id.ToString();
            }
            else
            {
                _logger.LogError($"ClickService: ReceiptCreate error -> {response.StatusCode} -> {dataStr}");
                return null;
            }
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            return null;
        }
    }

    public async Task<bool> ReceiptFiscalize(Subscription subscription, long payment_id)
    {
        try
        {
            var model = new ClickFiscalizeModel
            {
                service_id = _settings.ServiceId,
                payment_id = payment_id,
                items = new List<ClickItemModel>
                    {
                        new ClickItemModel
                        {
                            Name = subscription.NameRu,
                        Price = subscription.Price * 100,
                        Amount = 1,
                        SPIC = subscription.MXIK,
                        VATPercent = subscription.Vat,
                        VAT = subscription.Price * subscription.Vat,
                        PackageCode = subscription.PackageCode,
                        CommissionInfo = new ClickCommissionInfo { TIN = _settings.Tin }
                        }
                    },
                received_card = subscription.Price * 100,
            };

            var http = GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"ClickService: ReceiptFiscalize start -> {json}");
            var response = await http.PostAsync($"{_settings.Baseurl}/payment/ofd_data/submit_items", new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"ClickService: ReceiptFiscalize result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                var data_result = JsonConvert.DeserializeObject<ClickReceiptResult>(dataStr);

                if (data_result.error_code != 0 || !string.IsNullOrWhiteSpace(data_result.error_note))
                {
                    _logger.LogError($"PaymeService: ReceiptFiscalize error -> {response.StatusCode} -> {data_result.error_note} -> {data_result.error_code}");
                    return false;
                }

                else return true;
            }
            else
            {
                _logger.LogError($"ClickService: ReceiptFiscalize error -> {response.StatusCode} -> {dataStr}");
                return false;
            }
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            return false;
        }
    }

    public async Task<ClickReceiptStatus> ReceiptStatus(string invoice_id)
    {
        try
        {
            var http = GetClient();

            _logger.LogWarning($"ClickService: ReceiptStatus start -> {invoice_id}");
            var response = await http.GetAsync($"{_settings.Baseurl}/invoice/status/{_settings.ServiceId}/{invoice_id}");
            string dataStr = response.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"ClickService: ReceiptStatus result -> {dataStr}");

            if (response.IsSuccessStatusCode)
            {
                var data_result = JsonConvert.DeserializeObject<ClickReceiptStatus>(dataStr);

                if (data_result.error_code != 0 || !string.IsNullOrWhiteSpace(data_result.error_note))
                {
                    _logger.LogError($"PaymeService: ReceiptStatus error -> {response.StatusCode} -> {data_result.error_note} -> {data_result.error_code}");
                    return null;
                }

                else return data_result;
            }
            else
            {
                _logger.LogError($"ClickService: ReceiptStatus error -> {response.StatusCode} -> {dataStr}");
                return null;
            }
        }
        catch (Exception error)
        {
            _logger.LogError(error, error.Message);
            return null;
        }
    }
}