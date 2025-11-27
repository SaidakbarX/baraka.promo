namespace baraka.promo.Models
{
    /// <typeparam name="T">Ответ</typeparam>
    public class ApiBaseResultModel<T> : ApiBaseResultModel
    {
        public ApiBaseResultModel(T? data)
        {
            Data = data;
            Success = true;
        }
        public ApiBaseResultModel(ErrorModel error) : base(error) { }
        public ApiBaseResultModel(T? data, ErrorModel error) : base(error)
        {
            Data = data;
        }

        public ApiBaseResultModel() { }
        /// <summary>
        /// Результат запроса. Если "success" равен "true", иначе резултат будет равен "null"
        /// </summary>
        public T? Data { get; set; }
    }
    public class ApiBaseResultModel
    {
        public ApiBaseResultModel()
        {
            Success = true;
        }
        public ApiBaseResultModel(ErrorModel error)
        {
            Error = error;
            Success = false;
        }
        public bool Success { get; set; }
        /// <summary>
        /// Ошибка запроса. Если "success" равен "false", иначе резултат будет равен "null"
        /// </summary>
        public ErrorModel Error { get; set; }
    }
    public class ErrorModel
    {
        public ErrorModel(string code, Dictionary<string, string> message, string? description = null)
        {
            Message = message;
            Description = description;
            //MessageRu = messageRu;
            //MessageEn = messageEn;
            Code = code;
        }

        public string Code { get; set; }
        public Dictionary<string, string> Message { get; set; }
        public string? Description { get; set; }
        //public string Message { get; set; }
        //public string MessageRu { get; set; }
        //public string MessageEn { get; set; }
    }
}