using Newtonsoft.Json;

namespace baraka.promo.Models.PaymeModels
{
    public class PaymeReceiptCreate
    {
        public string id { get; set; }
        public string method { get; set; } = "receipts.create";

        [JsonProperty("params")]
        public ParamModel Params { get; set; } = new ParamModel();
        public class ParamModel
        {
            [JsonProperty("amount")]
            public decimal Amount { get; set; }

            [JsonProperty("account")]
            public AccountModel Account { get; set; } = new AccountModel();
            public class AccountModel
            {
                [JsonProperty("OrderId")]
                public Guid OrderId { get; set; }
            }

            [JsonProperty("detail")]
            public DetailModel Detail { get; set; } = new DetailModel();

            public class DetailModel
            {
                [JsonProperty("receipt_type")]
                public int ReceiptType { get; set; }

                [JsonProperty("items")]
                public List<ItemModel> Items { get; set; }
                public class ItemModel
                {
                    [JsonProperty("title")]
                    public string Title { get; set; }
                    [JsonProperty("price")]
                    public decimal Price { get; set; }
                    [JsonProperty("count")]
                    public int Count { get; set; }

                    [JsonProperty("code")]
                    public string Code { get; set; }
                    [JsonProperty("vat_percent")]
                    public int VatPercent { get; set; }
                    [JsonProperty("package_code")]
                    public string PackageCode { get; set; }
                    //[JsonProperty("units")]
                    //public string Units { get; set; }
                }
            }
        }
    }

    public class PaymeReceiptSend
    {
        public string id { get; set; }
        public string method { get; set; } = "receipts.send";

        [JsonProperty("params")]
        public ParamModel Params { get; set; } = new ParamModel();
        public class ParamModel
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }
        }
    }
}
