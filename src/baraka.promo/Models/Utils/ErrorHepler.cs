using Newtonsoft.Json;
using baraka.promo.Models;

namespace baraka.promo.Utils
{
    public static class ErrorHepler
    {
        public static ErrorModel GetError(ErrorHeplerType code, string? description = null, params object[] valueParams)
        {
            if (DictionaryHelper.TryGetValue(code, out Dictionary<string, string>? value))
            {
                if (valueParams != null)
                {
                    var valueStr = JsonConvert.SerializeObject(value);
                    var valueClone = JsonConvert.DeserializeObject<Dictionary<string, string>>(valueStr);
                    if (valueClone != null)
                    {
                        foreach (var item in valueClone)
                        {
                            valueClone[item.Key] = string.Format(valueClone[item.Key], valueParams);
                        }

                        return new ErrorModel(code.ToString(), valueClone, description);
                    }
                }
                return new ErrorModel(code.ToString(), value, description);
            }
            else
                return new ErrorModel(code.ToString(), null, description);


        }
    }

    

    public enum ErrorHeplerType
    {
        ERROR_PHONE_EXISTS,
        ERROR_PHONE_EMPTY,
        ERROR_PHONE_INCORRECT,
        ERROR_PHONE_NOT_FOUND,
        ERROR_INTERNAL,
        ERROR_SMS_SEND,
        ERROR_NO_COMFIRM_CODE_ENTERED,
        ERROR_INVALID_CONFIRMATION_CODE,
        ERROR_TIMEOUT_FOR_CONFIRM_CODE,

        ERROR_FILE_SIZE,
        ERROR_USER_NOT_FOUND,
        ERROR_USER_NOT_FOUND2,
        ERROR_USER_PHONE_NOT_FOUND,
        ERROR_USER_NOT_IDENTIFIED,
        ERROR_USER_CHANGEPASSWORD,

        ERROR_CONTRACT_NOT_FOUND,
        ERROR_CONTRACT_BALANCE_NEGATIVE,
        ERROR_DUPLICATE_CARD,
        ERROR_CARD_NOT_FOUND,
        ERROR_INVALID_CLIENT_BALANCE,
        ERROR_INVALID_CARD_BALANCE,

        ERROR_CONNECTOR_NOT_FOUND,
        ERROR_CONNECTOR_IS_OCCUPIED,
        ERROR_CONNECTOR_IS_UNAVAILABLE,
        ERROR_CONNECTOR_IS_FAULTED,
        ERROR_CONNECTOR_NOT_INSERTED,
        ERROR_CONNECTOR_IS_BUSY_NOW,

        ERROR_TRANSACTION_NOT_FOUND,
        ERROR_TRANSACTION_DUPLICATE,

        ERROR_ACCESS_DENIED,
        ERROR_INVALID_BALANCE_FOR_DELETE_CLIENT,
        ERROR_HAS_ACTIVE_SESSION_BALANCE_FOR_DELETE_CLIENT,

        ERROR_INCORRECT_PARAM,
        ERROR_REPEATED_REQUEST,

        ERROR_REQUEST_DUPLICATE,
        ERROR_REQUEST_NOT_FOUND,

        ERROR_USER_FIELDS_EMPTY,

        ERROR_ACCOUNTS_NOT_FOUND,
        ERROR_ACCOUNT_NOT_SET,
        ERROR_PROJECT_NOT_FOUND,
        ERROR_PAYMENT_NOT_FOUND,

        ERROR_AMOUNT_MUST_BE_GREATER_THAN_ZERO,

        ERROR_CHANGES_NOT_SAVED,
        ERROR_PROMO_NOT_FOUND,
        ERROR_PROMO_MAX_USED_OR_NOT_FOUND,

        ERROR_PROMO_REQUIREMENTS,

        ERROR_PROMO_REGION,
        ERROR_PROMO_RESTAURANT,
        ERROR_PROMO_ENOUGH_AMOUNT,
        ERROR_PROMO_NOT_STARTED,
        ERROR_PROMO_ENDED,
        ERROR_PROMO_USEDCOUNT,
        ERROR_PROMO_NOT_PERSONAL,

        ERROR_MODEL_EMPTY,

        ERROR_UNAUTHORIZED,
        ERROR_INVALID_NUMBER,


        //new
        ERROR_NOT_FOUND
    }
}