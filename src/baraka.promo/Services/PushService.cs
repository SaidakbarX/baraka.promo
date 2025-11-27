using baraka.promo.Delivery;
using baraka.promo.Models.PushModels;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace baraka.promo.Services;

public class PushService
{
    readonly PushSettingsModel _settings;
    readonly ILogger<PushService> _logger;
    readonly IHttpClientFactory _httpClientFactory;
    private readonly string jsonPath = "FirebaseInfo.json";
    public PushService(IOptions<PushSettingsModel> settings, ILogger<PushService> logger, IHttpClientFactory httpClientFactory)
    {
        _settings = settings != null ? settings.Value : new PushSettingsModel();
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    //public async Task Send(List<CustomerDevice> devices, string message, string objectId, int status)
    //{
    //    var types = devices.GroupBy(g => g.Type).ToList();
    //    foreach (var item in types)
    //    {
    //        switch (item.Key)
    //        {
    //            case DeviceType.IOS:
    //            case DeviceType.Android:
    //                await SendAndroid(item.Select(s => s.DeviceId).ToArray(), message, objectId, status);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    public async Task Send(List<CustomerDevice> devices, string message, string objectId, int status)
    {
        if (!string.IsNullOrEmpty(_settings.ProjectId))
        {
            int errorCount = 0;
            foreach (var device in devices)
            {
                if (errorCount >= 3) break;

                var result = await SendAndroid(device.DeviceId, message, objectId, status);

                if (result) errorCount = 0;
                else errorCount++;
            }
        }
    }

    async Task<bool> SendAndroid(string deviceId, string body, string objectId, int status)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();

            if (accessToken != null)
            {
                var message = new
                {
                    message = new
                    {
                        token = deviceId,
                        notification = new
                        {
                            title = _settings.Title,
                            body = body
                        },
                        data = new
                        {
                            //status = status.ToString(),
                            //objectId = objectId,
                            _settings.Title,
                        },
                    }
                };

                var jsonMessage = JsonConvert.SerializeObject(message);

                _logger.LogWarning($"PushService SendAndroid start -> {jsonMessage}");

                var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send")
                {
                    Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json")
                };

                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                var client = _httpClientFactory.CreateClient();
                var response = await client.SendAsync(request);
                string dataStr = response.Content.ReadAsStringAsync().Result;
                _logger.LogWarning($"PushService SendAndroid result -> {response} -> {dataStr}");

                if (response.IsSuccessStatusCode) return true;
                else return false;
            }

            return false;
        }
        catch (Exception error)
        {
            _logger.LogError(error, "DriverPushSender SendAndroid");
            return false;
        }
    }

    async Task<string> GetAccessTokenAsync()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), jsonPath);
        if (File.Exists(filePath))
        {
            GoogleCredential credential;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                                             .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
            }
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
        else return null;
    }
}