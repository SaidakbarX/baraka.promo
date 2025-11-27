namespace baraka.promo.Models.LoyaltyApiModels
{
    public class AuthorizeModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}
