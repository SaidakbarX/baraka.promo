using baraka.promo.Models.Enums;
using baraka.promo.Utils;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace baraka.promo.Utils
{
    public class DictionaryHelper
    {
        public static readonly Dictionary<ErrorHeplerType, Dictionary<string, string>> words = new();

        static DictionaryHelper()
        {
            ///// ERROR BEGIN///
            words.Add(ErrorHeplerType.ERROR_PHONE_EXISTS, new Dictionary<string, string> {
                {"uz","Bu telefon raqami roʻyxatdan oʻtgan"},
                {"ru","Телефон уже зарегистрирован" },
                {"en","Phone is already registered" }
            });
            words.Add(ErrorHeplerType.ERROR_PHONE_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Bu telefon raqami roʻyxatdan oʻtmagan"},
                {"ru","Телефон не зарегистрирован" },
                {"en","Phone not registered" }
            });
            words.Add(ErrorHeplerType.ERROR_INTERNAL, new Dictionary<string, string> {
                {"uz","Tizim xatosi"},
                {"ru","Системная ошибка" },
                {"en","System error" }
            });
            words.Add(ErrorHeplerType.ERROR_SMS_SEND, new Dictionary<string, string> {
                {"uz","SMS yuborishda xato"},
                {"ru","Ошибка при отравки смс" },
                {"en","Error when sending SMS" }
            });
            words.Add(ErrorHeplerType.ERROR_NO_COMFIRM_CODE_ENTERED, new Dictionary<string, string> {
                {"uz","Tasdiqlash kodi kiritilmagan"},
                {"ru","Не введен код подтверждения" },
                {"en","No comfirm code  entered" }
            });
            words.Add(ErrorHeplerType.ERROR_INVALID_CONFIRMATION_CODE, new Dictionary<string, string> {
                {"uz","Tasdiqlash kodi noto'g'ri"},
                {"ru","Неверный код подтверждения" },
                {"en","Invalid confirmation code" }
            });
            words.Add(ErrorHeplerType.ERROR_TIMEOUT_FOR_CONFIRM_CODE, new Dictionary<string, string> {
                {"uz","Tasdiqlash kodi uchun vaqt muddati o'tgan"},
                {"ru","Истекло время ожидания кода подтверждения" },
                {"en","Timeout for confirmation code" }
            });
            words.Add(ErrorHeplerType.ERROR_PHONE_EMPTY, new Dictionary<string, string> {
                {"uz","Telefon raqamini kiriting"},
                {"ru","Введите номер телефона" },
                {"en","Enter a phone number" }
            });
            words.Add(ErrorHeplerType.ERROR_PHONE_INCORRECT, new Dictionary<string, string> {
                {"uz","Telefon raqami noto'g'ri kiritilgan"},
                {"ru","Некорректный номер телефона" },
                {"en","Invalid phone number" }
            });
            words.Add(ErrorHeplerType.ERROR_INCORRECT_PARAM, new Dictionary<string, string> {
                {"uz","Noto'g'ri parametr {0}"},
                {"ru","Неверный параметр {0}" },
                {"en","Incorrect param {0}" }
            });
            words.Add(ErrorHeplerType.ERROR_FILE_SIZE, new Dictionary<string, string> {
                {"uz","Faylning maksimal hajmi {0} MB dan oshmasligi kerak"},
                {"ru","Максимальный размер файла не должно превышать {0} мб" },
                {"en","The maximum file size should not exceed {0} MB" }
            });
            words.Add(ErrorHeplerType.ERROR_USER_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Telefon raqami yoki parol noto'g'ri"},
                {"ru","Неправильное номер телефона или пароль" },
                {"en","Incorrect phone number or password" }
            });
            words.Add(ErrorHeplerType.ERROR_USER_NOT_FOUND2, new Dictionary<string, string> {
                {"uz","Foydalanuvchi topilmadi"},
                {"ru","Пользователь не найден" },
                {"en","User is not found" }
            });
            words.Add(ErrorHeplerType.ERROR_USER_PHONE_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Telefon raqami noto'g'ri"},
                {"ru","Неправильное номер телефона" },
                {"en","Incorrect phone number" }
            });
            words.Add(ErrorHeplerType.ERROR_USER_CHANGEPASSWORD, new Dictionary<string, string> {
                {"uz","Parolni oʻzgartirishda xatolik yuz berdi"},
                {"ru","Ошибка при изменении пароля" },
                {"en","Error changing password" }
            });
            words.Add(ErrorHeplerType.ERROR_DUPLICATE_CARD, new Dictionary<string, string> {
                {"uz","Karta allaqachon qo'shilgan"},
                {"ru","Карта уже добавлена" },
                {"en","Card already added" }
            });
            words.Add(ErrorHeplerType.ERROR_CARD_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Karta topilmadi"},
                {"ru","Карта не найдена" },
                {"en","Card not found" }
            });
            words.Add(ErrorHeplerType.ERROR_INVALID_CARD_BALANCE, new Dictionary<string, string> {
                {"uz","Kartada mablag' etarli emas"},
                {"ru","На карте недостаточно средств" },
                {"en","There are not enough funds on the card" }
            });
            words.Add(ErrorHeplerType.ERROR_ACCESS_DENIED, new Dictionary<string, string> {
                {"uz","Ruxsat berilmadi"},
                {"ru","Доступ запрещен" },
                {"en","Access denied" }
            });
            words.Add(ErrorHeplerType.ERROR_REQUEST_DUPLICATE, new Dictionary<string, string> {
                {"uz","Foydalanuvchida zayavka mavjud"},
                {"ru","У пользователя существует заявка" },
                {"en","The user has a request" }
            });

            words.Add(ErrorHeplerType.ERROR_REQUEST_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Zayavka topilmadi"},
                {"ru","Заявка не найдена" },
                {"en","Request not found" }
            });

            words.Add(ErrorHeplerType.ERROR_REPEATED_REQUEST, new Dictionary<string, string> {
                {"uz","Takroriy so'rov"},
                {"ru","Повторный запрос" },
                {"en","Repeated request" }
            });

            words.Add(ErrorHeplerType.ERROR_USER_FIELDS_EMPTY, new Dictionary<string, string> {
                {"uz","Foydalanuvchi maydonlari tanlanmagan"},
                {"ru","Пользовательские поля не выбраны" },
                {"en","User fields not selected" }
            });

            words.Add(ErrorHeplerType.ERROR_ACCOUNTS_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Hisoblar topilmadi"},
                {"ru","Аккаунты не найдены" },
                {"en","Accounts not found" }
            });


            words.Add(ErrorHeplerType.ERROR_ACCOUNT_NOT_SET, new Dictionary<string, string> {
                {"uz","Hisob o‘rnatilmagan"},
                {"ru","Аккаунт не установлен" },
                {"en","Account not set" }
            });

            words.Add(ErrorHeplerType.ERROR_USER_NOT_IDENTIFIED, new Dictionary<string, string> {
                {"uz","Foydalanuvchining shaxsi aniqlanmagan"},
                {"ru","Пользователь не идентифицирован" },
                {"en","User not Identified" }
            });

            words.Add(ErrorHeplerType.ERROR_PROJECT_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Loyiha topilmadi"},
                {"ru","Проект не найдены" },
                {"en","Project not found" }
            });

            words.Add(ErrorHeplerType.ERROR_PAYMENT_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Toʻlov topilmadi"},
                {"ru","Оплата не найдена" },
                {"en","Payment not found" }
            });

            words.Add(ErrorHeplerType.ERROR_AMOUNT_MUST_BE_GREATER_THAN_ZERO, new Dictionary<string, string> {
                {"uz","Toʻlov miqdori noldan katta bo'lishi kerak"},
                {"ru","Сумма должна быть больше нуля" },
                {"en","The amount must be greater than zero" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_MAX_USED_OR_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Bu promo allaqachon maksimal foydalanilgan Yoki siz ushbu promodan foydalana olmaysiz!"},
                {"ru","Эта акция уже использована максимально или вы не можете использовать эту акцию!" },
                {"en","This promo already Maximum used Or You can't use this Promo!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_NOT_FOUND, new Dictionary<string, string> {
                {"uz","Bu promo mavjud emas!"},
                {"ru","Промокод не найден!" },
                {"en","This promo is not available!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_NOT_PERSONAL, new Dictionary<string, string> {
                {"uz","Bu promo auditoriyasi shaxsiy emas!"},
                {"ru","Аудитория промокода не персональная!" },
                {"en","The audience of the promo is not personal!" }
            });

            words.Add(ErrorHeplerType.ERROR_CHANGES_NOT_SAVED, new Dictionary<string, string> {
                {"uz","O'zgarishlar saqlanmadi. So'rov modeli null bo'lishi mumkin emas! Qayta urinib ko'ring!"},
                {"ru","Изменения не сохранено. Модель запроса не может быть нулевой! Попробуйте еще раз!" },
                {"en","Changes not saved. Request model can't be null! Try again!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_REQUIREMENTS, new Dictionary<string, string> {
                {"uz","Aksiyadan foydalanish talablari bajarilmagan!"},
                {"ru","Не выполнены требования для использования промо!" },
                {"en","The requirements for using the promo have not been met!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_REGION, new Dictionary<string, string> {
                {"uz","Promo ushbu shaharda mavjud emas!"},
                {"ru","Акция недоступна в этом регионе!" },
                {"en","The Promo is not available in this region!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_RESTAURANT, new Dictionary<string, string> {
                {"uz","Promo bu restoranda mavjud emas!"},
                {"ru","Акция недоступна в этом ресторане!" },
                {"en","The Promo is not available in this restaurant!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_ENOUGH_AMOUNT, new Dictionary<string, string> {
                {"uz","Buyurtma miqdori reklama kodining mezonlariga mos kelmaydi!"},
                {"ru","Сумма заказа не соответствует критериям промокода!" },
                {"en","The order amount does not meet the criteria for the promocode!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_NOT_STARTED, new Dictionary<string, string> {
                {"uz","Promo-kodning boshlanish sanasi kelmadi!"},
                {"ru","Дата начало промокода не настал!" },
                {"en","Promo code start date has not arrived!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_ENDED, new Dictionary<string, string> {
                {"uz","Promo-kod muddati tugagan!"},
                {"ru","Дата использование промокода истекла!" },
                {"en","The promotional code has expired!" }
            });

            words.Add(ErrorHeplerType.ERROR_PROMO_USEDCOUNT, new Dictionary<string, string> {
                {"uz","Bu promo allaqachon foydalanilgan!"},
                {"ru","Эта акция уже использована!" },
                {"en","This promo already used!" }
            });

            words.Add(ErrorHeplerType.ERROR_UNAUTHORIZED, new Dictionary<string, string> {
                 { "uz", "Siz ruxsat etilmagan foydalanuvchisiz!" },
                 { "ru", "Вы неавторизованный пользователь!" },
                 { "en", "You are not authorized!" }
            });

            words.Add(ErrorHeplerType.ERROR_NOT_FOUND, new Dictionary<string, string> {
                 { "uz", "Topilmadi!" },
                 { "ru", "Не найдено!" },
                 { "en", "Not found!" }
            });

            ///// ERROR END///
        }


        public static bool TryGetValue(ErrorHeplerType key, [MaybeNullWhen(false)] out Dictionary<string, string> value)
        {
            if (words.TryGetValue(key, out value))
            {
                var normalized = new Dictionary<string, string>(
                    value.ToDictionary(k => k.Key.ToLower(), v => v.Value),
                    StringComparer.OrdinalIgnoreCase
                );

                value = normalized;
                return true;
            }
            return false;
        }


        public static string GetMessage(ErrorHeplerType key, string lang = "uz")
        {
            try
            {
                if (TryGetValue(key, out var dict) && dict != null)
                {
                    lang = lang.ToLower();

                    if (dict.TryGetValue(lang, out var message) && !string.IsNullOrWhiteSpace(message))
                        return message;
                }

                return "Xatolik aniqlanmadi";
            }
            catch (Exception ex)
            {
                // Debug uchun logga yozish mumkin
                Console.WriteLine($"GetMessage error: {ex.Message}");
                return "Xatolik aniqlanmadi";
            }
        }

        public static Dictionary<string, string> GetValueOrDefault(ErrorHeplerType key, params object[] valueParams)
        {
            try
            {
                if (words.TryGetValue(key, out Dictionary<string, string>? value) && value != null)
                {
                    var cloned = new Dictionary<string, string>(value);

                    if (valueParams != null && valueParams.Length > 0)
                    {
                        foreach (var item in cloned.Keys.ToList())
                        {
                            try
                            {
                                cloned[item] = string.Format(cloned[item], valueParams);
                            }
                            catch
                            {

                            }
                        }
                    }

                    return cloned;
                }

                return new Dictionary<string, string>
        {
            { "uz", "Xatolik aniqlanmadi" },
            { "ru", "Ошибка не определена" },
            { "en", "Error not defined" }
        };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetValueOrDefault error: {ex.Message}");
                return new Dictionary<string, string>
        {
            { "uz", "Xatolik aniqlanmadi" },
            { "ru", "Ошибка не определена" },
            { "en", "Error not defined" }
        };
            }
        }


        public static string GetErrorString(ErrorHeplerType key, Language language = Language.Ru, params object[] valueParams)
        {
            try
            {
                if (words == null)
                    return string.Empty;

                if (!words.TryGetValue(key, out Dictionary<string, string>? value) || value == null)
                    return string.Empty;

                var resultDict = new Dictionary<string, string>(value, StringComparer.OrdinalIgnoreCase);

                if (valueParams != null && valueParams.Length > 0)
                {
                    foreach (var item in resultDict.Keys.ToList())
                    {
                        resultDict[item] = string.Format(resultDict[item], valueParams);
                    }
                }

                var langKey = language.ToString().ToLower();

                if (resultDict.TryGetValue(langKey, out string? message))
                    return message ?? string.Empty;

                return resultDict.Values.FirstOrDefault() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }



    }
}