using baraka.promo.Pages.TgPushSender;
using baraka.promo.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace baraka.promo.Pages.Basepage
{
    public class BaseRazorComponent : ComponentBase
    {


        [Inject]
        public NavigationManager? NavManager { get; set; }
        [Inject]
        public IJSRuntime? jsRuntime { get; set; }
        [Inject]
        public NotificationService? NotificationService { get; set; }

        public string searchTextValue = "";
        public int _pageIndex = 1;
        public int _pageSize = 10;
        //public int _pageSkip { get { return (_pageIndex - 1) * _pageSize; } }
        public int _pageSkip;
        public int _total = 0;
        public bool _loading = true;
        public bool actionLoading;

        public ObservableCollection<NotificationMessage> Messages { get; private set; } = new ObservableCollection<NotificationMessage>();



        protected override Task OnInitializedAsync()
        {
            LoadData();
            return base.OnInitializedAsync();
        }

        protected virtual void LoadData()
        {
        }
        protected virtual void OnSearch(string value)
        {
            if (searchTextValue == value) return;
            searchTextValue = value;
            //_loading = true;
            //StateHasChanged();
            _pageIndex = 1;
            LoadData();
            //_loading = false;
            //StateHasChanged();
        }
        public void GoTo(string url, bool forceReload = false, bool isNewTab = false)
        {
            if (!isNewTab && NavManager != null)
                NavManager.NavigateTo(url, forceReload);
            else if (jsRuntime != null)
                _ = jsRuntime.InvokeVoidAsync("open", url, "_blank");

        }

        public void PageIndexChanged(PagerEventArgs args)
        {
            _pageIndex = args.PageIndex;
            _pageSize = args.Top;
            _pageSkip = args.Skip;
            StateHasChanged();
            Console.WriteLine(_pageSkip);
            LoadData();
        }

        protected virtual void ClearAndLoadData()
        {
            searchTextValue = "";
            _pageIndex = 1;
            _total = 0;
            LoadData();
        }

        public void ChangeStates()
        {
            StateHasChanged();
        }

        public string PhoneToDisplay(string? Phone)
        {
            try
            {
                if (Phone != null)
                {
                    string phone = Phone.Replace("+", "");
                    return $"+{phone[0..3]} ({phone[3..5]}) {phone[5..8]} {phone[8..10]} {phone[10..12]}";
                }
                else
                    return "---";
            }
            catch (Exception)
            {
                return Phone ?? "---";
            }
        }

        public string MoneyToDisplay(decimal? price, bool withMinus = false, bool divideTo100 = false)
        {
            return StringHelper.MoneyToDisplay(price, withMinus, divideTo100);
        }

        public string DateToDisplay(DateTime? dateTime, bool withHour = false)
        {
            return StringHelper.DateToDisplay(dateTime, withHour);
        }
        public string DateToDisplay(string? dateTime, bool withHour = false)
        {
            return StringHelper.DateToDisplay(dateTime, withHour);
        }
        public string DateOnlyDisplay(DateTime? dateTime, bool withHour = false)
        {
            return StringHelper.DateOnlyDisplay(dateTime, withHour);
        }
        public string CardNumber(string? card)
        {
            return StringHelper.CardNumber(card);
        }


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

        public void ErrorNotify(string summary = "Ошибка", string detail = "Что-то пошло не так", NotificationSeverity severity = NotificationSeverity.Error, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
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
        public void ErrorNotify(Exception e, NotificationSeverity severity = NotificationSeverity.Error, double duration = 3000.0, Action<NotificationMessage> click = null, bool closeOnClick = false, object payload = null, Action<NotificationMessage> close = null)
        {
            NotificationMessage item = new NotificationMessage
            {
                Duration = duration,
                Severity = severity,
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
    }
}
