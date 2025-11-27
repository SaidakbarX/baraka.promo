using Newtonsoft.Json;

namespace baraka.promo.Models.PaymeModels
{
    public class PaymeReceiptCheckResult
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }

        [JsonProperty("result")]
        public ResultModel Result { get; set; }
        public class ResultModel
        {
            [JsonProperty("state")]
            public PaymeReceiptStatus State { get; set; }
        }
    }
}
