using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmsSender.Serivce.Baraka.Promo;
using SmsSender.Serivce.Models;
using SmsSender.Serivce.Models.TgMessageModel;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;
namespace SmsSender.Serivce.Services
{
    public class SenderService : ISenderService
    {
        readonly string tgBotToken;
        private readonly IServiceScopeFactory _factoryService;
        private readonly ILogger<SenderService> _logger;
        readonly PushSettingsModel _settings;

        readonly IHttpClientFactory _httpClientFactory;
        readonly string jsonPath = "FirebaseInfo.json";

        public SenderService(IConfiguration configuration, IServiceScopeFactory factoryService,
            ILogger<SenderService> logger, IHttpClientFactory httpClientFactory)
        {
            _factoryService = factoryService;
            tgBotToken = configuration.GetSection("BotToken").Value;
            _logger = logger;
            _settings = new PushSettingsModel();
            _settings.ProjectId = configuration.GetSection("PushProjectId").Value;
            _settings.Title = configuration.GetSection("PushTitle").Value;
            _httpClientFactory = httpClientFactory;
        }
        public async Task SendData()
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<PromoDbContext>();
                var currentTime = DateTime.Now;

                //var messageHeader = _db.MessageHeaders.OrderBy(o => o.StartDate).FirstOrDefault(ms =>
                //    ms.Status == MessageHeaderStatus.Start &&
                //    !ms.IsDeleted &&
                //    (ms.StartDate <= currentTime &&
                //    ((ms.WorkingTimeFrom == DateTime.MinValue && ms.WorkingTimeTo == DateTime.MinValue) ||
                //    (ms.WorkingTimeFrom.TimeOfDay <= currentTime.TimeOfDay && ms.WorkingTimeTo.TimeOfDay >= currentTime.TimeOfDay))));

                var messageHeader = _db.MessageHeaders
                .Where(ms => ms.Status == MessageHeaderStatus.Start &&
                 !ms.IsDeleted &&
                 ms.StartDate <= currentTime &&
                 (
                     // Если указано минимальное время (нулевые значения)
                     (ms.WorkingTimeFrom == DateTime.MinValue && ms.WorkingTimeTo == DateTime.MinValue) ||

                     // Если время в стандартном интервале (обычное рабочее время)
                     (ms.WorkingTimeFrom.TimeOfDay <= currentTime.TimeOfDay &&
                      ms.WorkingTimeTo.TimeOfDay >= currentTime.TimeOfDay) ||

                     // Если время переходит через полночь (например, с 23:00 до 03:00)
                     (ms.WorkingTimeFrom.TimeOfDay > ms.WorkingTimeTo.TimeOfDay &&
                      (currentTime.TimeOfDay >= ms.WorkingTimeFrom.TimeOfDay ||
                       currentTime.TimeOfDay <= ms.WorkingTimeTo.TimeOfDay))
                 ))
                .OrderBy(o => o.StartDate)
                .FirstOrDefault();

                if (messageHeader == null)
                {
                    //_logger.LogWarning($"SenderService.SendData messageHeader is null");
                    return;
                }

                var messages = _db.Messages.Where(w => w.Status == Status.New && w.MessageHeaderId == messageHeader.Id).Take(10000).ToList();

                _logger.LogWarning($"SenderService.SendData start messageHeaderId: {messageHeader.Id}, messagesCount: {messages.Count}");

                Parallel.ForEach(messages, new ParallelOptions
                {
                    MaxDegreeOfParallelism = 20
                }, async delegate (Data.Message msg, ParallelLoopState state)
                {
                    Task.Delay(1000).Wait();
                    var sendResult = await SendToUser(msg.ChatId, messageHeader.Message, messageHeader.IsImage, messageHeader.FileId, messageHeader.JsonButtons);
                    msg.MessageId = sendResult?.IsSuccess == true ? sendResult.MessageId : 0;
                    msg.Status = sendResult?.IsSuccess == true ? Status.Sended : Status.Error;
                    msg.SendTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(sendResult?.Error))
                    {
                        msg.Error = sendResult?.Error;
                    }
                });

                _logger.LogWarning($"SenderService.SendData stop messageHeaderId: {messageHeader.Id}, messagesCount: {messages.Count}, SendedCount:{messages.Where(w => w.Status == Status.Sended).Count()}, ErrorCount:{messages.Where(w => w.Status == Status.Error).Count()}");

                await _db.SaveChangesAsync();

                if (!_db.Messages.Where(a => a.MessageHeaderId == messageHeader.Id).Any(a => a.Status == Status.New))
                {
                    messageHeader.Status = MessageHeaderStatus.Completed;
                    await _db.SaveChangesAsync();

                    _logger.LogWarning($"SenderService.SendData Completed messageHeaderId: {messageHeader.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SenderService.SendData error");
                //Console.WriteLine(ex.ToString());
            }
        }
        public async Task<MessageResultModel> SendToUser(long telegramId, string? message, bool? isPhoto, string? fileId, string? tgButtons)
        {
            var result = new MessageResultModel();
            try
            {
                IReplyMarkup? buttons = null;
                if (!string.IsNullOrEmpty(tgButtons))
                {
                    var list = JsonConvert.DeserializeObject<List<TgButtonModel>>(tgButtons);
                    buttons = getButtuns(list);
                }
                var botClient = new TelegramBotClient(tgBotToken);
                if (fileId == null)
                {
                    Message tgMessage;
                    tgMessage = await botClient.SendTextMessageAsync(telegramId, message, parseMode: ParseMode.Html, replyMarkup: buttons);
                    result.MessageId = tgMessage.MessageId;
                }
                else
                {
                    InputFile file = InputFile.FromFileId(fileId);
                    Message tgMessage;

                    if (isPhoto == true)
                    {
                        tgMessage = botClient.SendPhotoAsync(telegramId, file, caption: message, parseMode: ParseMode.Html, replyMarkup: buttons).Result;
                        if (string.IsNullOrEmpty(fileId))
                            fileId = tgMessage.Photo?.LastOrDefault()?.FileId;
                        result.MessageId = tgMessage.MessageId;
                    }
                    else
                    {
                        tgMessage = botClient.SendVideoAsync(telegramId, file, 0, 0, 0, caption: message, parseMode: ParseMode.Html, replyMarkup: buttons).Result;
                        if (string.IsNullOrEmpty(fileId))
                            fileId = tgMessage.Video?.FileId;
                        result.MessageId = tgMessage.MessageId;
                    }
                }

                result.IsSuccess = true;
            }
            catch (ApiRequestException tgError)
            {
                if (tgError.ErrorCode == 429)
                {
                    await SendToUser(telegramId, message, isPhoto, fileId, tgButtons);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SenderService.SendToUser error");
                result.Error = ex.Message;
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        private IReplyMarkup? getButtuns(List<TgButtonModel>? buttons)
        {
            if (buttons == null || buttons.Count() == 0) return null;

            List<InlineKeyboardButton> rowItems = new List<InlineKeyboardButton>();
            foreach (var button in buttons)
            {
                InlineKeyboardButton ikb = new InlineKeyboardButton(button.Name);
                ikb.WebApp = new Telegram.Bot.Types.WebAppInfo();
                ikb.WebApp.Url = button.Url;
                rowItems.Add(ikb);
            }
            return new InlineKeyboardMarkup(rowItems);
        }

        public async Task<bool> SendPushData()
        {
            try
            {
                using var scope = _factoryService.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<PromoDbContext>();
                var currentTime = DateTime.Now;

                var messageHeader = _db.NotificationHeaders
                .Where(ms => ms.Status == MessageHeaderStatus.Start &&
                 !ms.IsDeleted &&
                 ms.StartDate <= currentTime &&
                 (
                     // Если указано минимальное время (нулевые значения)
                     (ms.WorkingTimeFrom == DateTime.MinValue && ms.WorkingTimeTo == DateTime.MinValue) ||

                     // Если время в стандартном интервале (обычное рабочее время)
                     (ms.WorkingTimeFrom.TimeOfDay <= currentTime.TimeOfDay &&
                      ms.WorkingTimeTo.TimeOfDay >= currentTime.TimeOfDay) ||

                     // Если время переходит через полночь (например, с 23:00 до 03:00)
                     (ms.WorkingTimeFrom.TimeOfDay > ms.WorkingTimeTo.TimeOfDay &&
                      (currentTime.TimeOfDay >= ms.WorkingTimeFrom.TimeOfDay ||
                       currentTime.TimeOfDay <= ms.WorkingTimeTo.TimeOfDay))
                 ))
                .OrderBy(o => o.StartDate)
                .FirstOrDefault();

                if (messageHeader == null)
                {
                    //_logger.LogWarning($"SenderService.SendPushData messageHeader is null");
                    return false;
                }

                var messages = _db.Notifications.Where(w => w.Status == Status.New && w.NotificationHeaderId == messageHeader.Id).Take(1000).ToList();

                _logger.LogWarning($"SenderService.SendPushData start messageHeaderId: {messageHeader.Id}, messagesCount: {messages.Count}");

                //Parallel.ForEach(messages, new ParallelOptions
                //{
                //    MaxDegreeOfParallelism = 20
                //},  delegate (Notification msg, ParallelLoopState state)
                //{
                //    Task.Delay(1000).Wait();

                //    bool is_push = false;
                //    if(msg.Language == Language.Uz) is_push = SendAndroid(msg.DeviceId, messageHeader.MessageUz, null, 0).Result;
                //    else if (msg.Language == Language.Ru) is_push = SendAndroid(msg.DeviceId, messageHeader.MessageRu, null, 0).Result;
                //    else if (msg.Language == Language.En) is_push = SendAndroid(msg.DeviceId, messageHeader.MessageEn, null, 0).Result;
                //    else if (msg.Language == Language.Kz) is_push = SendAndroid(msg.DeviceId, messageHeader.MessageKz, null, 0).Result;

                //    msg.Status = is_push ? Status.Sended : Status.Error;
                //    msg.SendTime = DateTime.Now;
                //});

                foreach (var msg in messages)
                {
                    Task.Delay(1000).Wait();

                    bool is_push = false;
                    if (msg.Language == Language.Uz) is_push = await SendAndroid(msg.DeviceId, messageHeader.MessageUz, null, 0);
                    else if (msg.Language == Language.Ru) is_push = await SendAndroid(msg.DeviceId, messageHeader.MessageRu, null, 0);
                    else if (msg.Language == Language.En) is_push = await SendAndroid(msg.DeviceId, messageHeader.MessageEn, null, 0);
                    else if (msg.Language == Language.Kz) is_push = await SendAndroid(msg.DeviceId, messageHeader.MessageKz, null, 0);

                    msg.Status = is_push ? Status.Sended : Status.Error;
                    msg.SendTime = DateTime.Now;
                }

                _logger.LogWarning($"SenderService.SendPushData stop messageHeaderId: {messageHeader.Id}, messagesCount: {messages.Count}, SendedCount:{messages.Where(w => w.Status == Status.Sended).Count()}, ErrorCount:{messages.Where(w => w.Status == Status.Error).Count()}");

                await _db.SaveChangesAsync();

                if (!_db.Notifications.Where(a => a.NotificationHeaderId == messageHeader.Id).Any(a => a.Status == Status.New))
                {
                    messageHeader.Status = MessageHeaderStatus.Completed;
                    await _db.SaveChangesAsync();

                    _logger.LogWarning($"SenderService.SendPushData Completed messageHeaderId: {messageHeader.Id}");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SenderService.SendPushData error");
                return false;
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
                    //_logger.LogWarning($"SenderService SendAndroid start -> {jsonMessage}");

                    var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{_settings.ProjectId}/messages:send")
                    {
                        Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json")
                    };

                    request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");

                    var client = _httpClientFactory.CreateClient();
                    var response = await client.SendAsync(request);
                    string dataStr = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode) return true;
                    else
                    {
                        _logger.LogError($"SenderService SendAndroid error -> {response.StatusCode} -> {dataStr}");
                        return false;
                    }
                }

                return false;
            }
            catch (Exception error)
            {
                _logger.LogError(error, $"SenderService SendAndroid error -> {error}");
                return false;
            }
        }

        async Task<string> GetAccessTokenAsync()
        {
            //var filePath = "C:\\inetpub\\baraka_promo_sender_iis\\FirebaseInfo.json";

            var filePath = Path.Combine(AppContext.BaseDirectory, jsonPath);

            _logger.LogWarning($"SenderService GetAccessTokenAsync -> {filePath}");

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
            else
            {
                _logger.LogError($"SenderService GetAccessTokenAsync file not found");
                return null;
            }
        }
    }


    public class TgUserModel
    {
        public string TelegramId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int Language { get; set; }
    }

    public class MessageResultModel
    {
        public bool IsSuccess { get; set; }
        public int MessageId { get; set; }
        public string? Error { get; set; }
    }
}
