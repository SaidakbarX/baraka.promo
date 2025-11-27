using baraka.promo.Models.MindBoxModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace baraka.promo.Services;

public class MindBoxService
{
    readonly MindBoxSettings _settings;
    readonly ILogger<MindBoxService> _logger;
    readonly IHttpClientFactory _httpClientFactory;

    public MindBoxService(IOptions<MindBoxSettings> options, ILogger<MindBoxService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settings = options != null ? options.Value : new MindBoxSettings();
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    async Task<HttpClient> GetClient()
    {
        var http = _httpClientFactory.CreateClient();
        http.Timeout = TimeSpan.FromSeconds(30);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        http.DefaultRequestHeaders.Add("Authorization", $"Mindbox secretKey=\"{_settings.Secret}\"");
        return http;
    }

    public async Task<bool> CreateTransaction(Guid transaction_id, string phone, decimal sum, string product_id)
    {
        try
        {
            MindBoxTransaction model = new MindBoxTransaction
            {
                pointOfContact = _settings.PointOfContact,
                customer = new CustomerModel
                {
                    mobilePhone = phone,
                    //ids = new Ids { }
                },
                order = new OrderModel
                {
                    totalPrice = sum,
                    mobilePhone = phone,
                    ids = new Ids
                    {
                        externalOrderId = transaction_id.ToString(),
                    },

                    lines = new List<Line>
                        {
                            new Line
                            {
                                basePricePerItem = sum,
                                quantity = 1,
                                lineNumber = 1,
                                product = new Product
                            {
                                ids = new Ids
                                {
                                    productExternalId = product_id
                                }
                            },
                            status = new Status
                            {
                                ids = new Ids
                                {
                                    externalId = "Create"
                                }
                            },
                            }
                        },

                }
            };

            var http = await GetClient();
            var json = JsonConvert.SerializeObject(model);

            _logger.LogWarning($"MindBoxService: CreateTransaction start -> {json}");
            var result = await http.PostAsync($"{_settings.Url}&operation=BeginAuthorizedOrderTransaction1BeginAuthorizedOrderTransactionNEW&transactionId={transaction_id}",
                                                new StringContent(json, Encoding.UTF8, "application/json"));
            string dataStr = result.Content.ReadAsStringAsync().Result;
            _logger.LogWarning($"MindBoxService: CreateTransaction result -> {dataStr}");

            if (result.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<MindBoxTransactionResult>(dataStr)?.Success ?? false;
            }
            else
            {
                _logger.LogError($"MindBoxService: CreateTransaction error -> {result.StatusCode} -> {dataStr}");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}