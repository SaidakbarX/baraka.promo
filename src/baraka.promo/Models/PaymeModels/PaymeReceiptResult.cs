using Newtonsoft.Json;

namespace baraka.promo.Models.PaymeModels
{
    public class PaymeReceiptResult
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }

        [JsonProperty("result")]
        public ResultModel Result { get; set; }
        public class ResultModel
        {
            [JsonProperty("receipt")]
            public ReceiptModel Receipt { get; set; }
            public class ReceiptModel
            {
                [JsonProperty("_id")]
                public string Id { get; set; }
                //[JsonProperty("create_time")]
                //public long CreateTime { get; set; }
                //[JsonProperty("pay_time")]
                //public long PayTime { get; set; }
                //[JsonProperty("cancel_time")]
                //public long CancelTime { get; set; }
                [JsonProperty("state")]
                public PaymeReceiptStatus State { get; set; }
                [JsonProperty("type")]
                public int Type { get; set; }
                [JsonProperty("external")]
                public bool External { get; set; }
                [JsonProperty("operation")]
                public int Operation { get; set; }
                [JsonProperty("description")]
                public string Description { get; set; }
                [JsonProperty("amount")]
                public int Amount { get; set; }
            }
        }

        [JsonProperty("error")]
        public ErrorModel Error { get; set; }

        public class ErrorModel
        {
            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
    
    public class PaymeReceiptSendResult
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }

        [JsonProperty("result")]
        public ResultModel Result { get; set; }
        public class ResultModel
        {
            [JsonProperty("success")]
            public bool Success { get; set; }
        }

        [JsonProperty("error")]
        public ErrorModel Error { get; set; }

        public class ErrorModel
        {
            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
