using baraka.promo.Models;
using Radzen;
using static baraka.promo.Core.GetSegments;

namespace baraka.promo.Services.HelperServices
{
    public class FrontResponseHandler
    {
        MyNotificationService notificationService;
        public FrontResponseHandler(MyNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }



        public bool Handle(ApiBaseResultModel? result, bool showError = true, bool showSuccess = false, string successMessage = "")
        {
            try
            {
                if (result != null)
                {
                    if (result.Success)
                    {
                        if (showSuccess || !string.IsNullOrEmpty(successMessage))
                            notificationService.NotifySuccess(successMessage);
                    }
                    else if (showError)
                        notificationService.ErrorNotify(result.Error);
                    return result.Success;
                }
                else
                    notificationService.ErrorNotify("Данные не найдены");
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
            }
            return result?.Success ?? false;
        }

        public Return Handle<Return>(Return returnV, ApiBaseResultModel<Return> result, bool showSuccess = false, string successMessage = "")
        {
            try
            {
                if (result.Success)
                {
                    if (result.Data == null)
                    {
                        notificationService.Notify("Данные не найдены");
                        return returnV;
                    }
                    else
                    {
                        if (showSuccess || !string.IsNullOrEmpty(successMessage))
                            notificationService.NotifySuccess(successMessage);
                        return result.Data;
                    }
                }
                else
                    notificationService.ErrorNotify(result.Error);
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
            }
            return returnV;
        }


        public ListBaseModel<Return> Handle<Return>(ListBaseModel<Return> returnV, ApiBaseResultModel<ListBaseModel<Return>> result, bool showSuccess = false, string successMessage = "")
        {
            try
            {
                if (result.Success)
                {
                    if (result.Data == null)
                    {
                        notificationService.Notify("Данные не найдены");
                        return returnV;
                    }
                    else
                    {
                        if (showSuccess || !string.IsNullOrEmpty(successMessage))
                            notificationService.NotifySuccess(successMessage);
                        return result.Data;
                    }
                }
                else
                    notificationService.ErrorNotify(result.Error);
            }
            catch (Exception e)
            {
                notificationService.ErrorNotify(e);
            }
            return returnV;
        }

    }
}
