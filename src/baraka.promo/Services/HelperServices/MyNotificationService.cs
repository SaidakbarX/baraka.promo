using baraka.promo.Models;
using baraka.promo.Models.Enums;
using baraka.promo.Pages.TgPushSender;
using baraka.promo.Utils;
using Radzen;
using System.Collections.ObjectModel;

namespace baraka.promo.Services.HelperServices
{
    public class MyNotificationService
    {
        NotificationService NotificationService { get; set; }

        public MyNotificationService(NotificationService NotificationService)
        {
            this.NotificationService = NotificationService;
        }


        public ObservableCollection<NotificationMessage> Messages { get; private set; } = new ObservableCollection<NotificationMessage>();

        public void Notify(string summary, string detail = "", NotificationSeverity severity = NotificationSeverity.Info, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };
            if (!Messages.Contains(item))
            {
                Messages.Add(item);
            }
            NotificationService?.Notify(item);
        }

        public void NotifySuccess(string summary = "Успешный", string detail = "", NotificationSeverity severity = NotificationSeverity.Success, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = severity,
                Summary = !string.IsNullOrEmpty(summary) ? summary : "Успешный",
                Detail = !string.IsNullOrEmpty(detail) ? detail : summary,
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };
            if (!Messages.Contains(item))
            {
                Messages.Add(item);
            }
            NotificationService?.Notify(item);
        }

        public void ErrorNotify(string summary = "Ошибка", string detail = "Что-то пошло не так",  double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = NotificationSeverity.Error,
                Summary = summary,
                Detail = detail,
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };
            if (!Messages.Contains(item))
            {
                Messages.Add(item);
            }
            NotificationService?.Notify(item);
        }
        public void ErrorNotify(Exception e, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = NotificationSeverity.Error,
                Summary = e.Message,
                Detail = e.InnerException?.Message,
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };
            if (!Messages.Contains(item))
            {
                Messages.Add(item);
            }
            NotificationService?.Notify(item);
        }
        public void ErrorNotify(ErrorModel e, Language lang = Language.Ru, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = NotificationSeverity.Error,
                Summary = e.Code.ToString(),
                Detail = DictionaryHelper.GetErrorString(EnumHelper<ErrorHeplerType>.StringToEnum(e.Code), lang),
                Click = click,
                Close = close,
                CloseOnClick = closeOnClick,
                Payload = payload
            };
            if (!Messages.Contains(item))
            {
                Messages.Add(item);
            }
            NotificationService?.Notify(item);
        }

    }
}
