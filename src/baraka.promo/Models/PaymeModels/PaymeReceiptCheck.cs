using Newtonsoft.Json;

namespace baraka.promo.Models.PaymeModels
{
    public class PaymeReceiptCheck
    {
        public string id { get; set; }
        public string method { get; set; } = "receipts.check";

        [JsonProperty("params")]
        public ParamModel Params { get; set; } = new ParamModel();
        public class ParamModel
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }
    }
}
