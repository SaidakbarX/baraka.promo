using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace baraka.promo.Models.LoyaltyApiModels
{
    public class AuthOptions
    {
        public const string ISSUER = "baraka-loyalty"; // издатель токена
        public const string AUDIENCE = "baraka-loyalty"; // потребитель токена
        const string KEY = "baraka_promo_loyalty_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 60 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
