namespace baraka.promo.Models.CardPaymentModels
{
    public class BarakaTokenModel
    {
        public string userName { get; set; }
        public string password { get; set; }
    }

    public class BarakaTokenResponse
    {
        public string token { get; set; }
    }
}
