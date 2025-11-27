using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace baraka.promo.Pages.TgPushSender
{
    public class TgHelper
    {
        private string connectionString;
        public TgHelper(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("TgBotConnection");
        }
        private static List<TgUserModel> tgUserModels = new List<TgUserModel>();
        public Task<int> GetClientCount(List<string> phones, bool uz, bool ru, bool en, bool forSegment)
        {
            var clients = GetClients(phones, uz, ru, en, forSegment).Result;
            return Task.FromResult(clients.Count);
        }       

        public Task<List<TgUserModel>> GetClients(List<string> phones, bool uz, bool ru, bool en, bool forSegment)
        {

            if (tgUserModels.Count() == 0)
                using (var conn = new SqlConnection(connectionString))
                {
                    var dt = new DataTable();
                    var sb = new StringBuilder();
                    sb.Append(@"SELECT [TelegramId],[Name],[Phone],[Language]
                          FROM [Users]
                          where IsDeleted=0 ");
                    //if (phones != null && phones.Count > 0)
                    //{
                    //    var phoneList = phones.Where(w => !w.StartsWith("+")).Select(s => $"+{s}").ToList();
                    //    phoneList.AddRange(phones);
                    //    sb.AppendLine($"AND Phone in ('{string.Join(",", phoneList).Replace(",", "','")}')");
                    //}

                    //var da = new SqlDataAdapter(sb.ToString(), conn);
                    //da.Fill(dt);

                    SqlCommand oCmd = new SqlCommand(sb.ToString(), conn);

                    conn.Open();
                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {
                        while (oReader.Read())
                        {
                            var user = new TgUserModel
                            {
                                TelegramId = oReader["TelegramId"] != DBNull.Value ? oReader["TelegramId"].ToString() : null,
                                Name = oReader["Name"] != DBNull.Value ? oReader["Name"].ToString() : null,
                                Phone = oReader["Phone"] != DBNull.Value ? oReader["Phone"].ToString() : null,
                                Language = oReader["Language"] != DBNull.Value ? (int)oReader["Language"] : 0,
                            };
                            tgUserModels.Add(user);
                        }

                        conn.Close();
                    }


                    //foreach (DataRow row in dt.Rows)
                    //{
                    //    var user = new TgUserModel
                    //    {
                    //        TelegramId = row["TelegramId"] != DBNull.Value ? row["TelegramId"].ToString() : null,
                    //        Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : null,
                    //        Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : null,
                    //        Language = row["Language"] != DBNull.Value ? (int)row["Language"] : 0,
                    //    };
                    //    tgUserModels.Add(user);
                    //}
                }
            List<TgUserModel> result = new List<TgUserModel>();
            result.AddRange(tgUserModels);
            if (phones != null && phones.Count > 0 || forSegment)
            {
                phones.AddRange(phones.Where(w => !w.StartsWith("+")).Select(s => $"+{s}").ToList());

                result = (from tgPhone in result
                          join dlPhone in phones on tgPhone.Phone equals dlPhone
                          select tgPhone).ToList();

                //result  = result.Where(w=>phones.Contains(w.Phone)).ToList();
            }
            List<int> langs = new List<int>();
            if (uz)
                langs.Add(1);
            if (ru)
                langs.Add(2);
            if (en)
                langs.Add(3);
            if (langs.Count > 0)
            {
                result = (from tgPhone in result
                          join lang in langs on tgPhone.Language equals lang
                          select tgPhone).ToList();
                //result = result.Where(w => langs.Contains(w.Language)).ToList(); 
            }

            result = result.DistinctBy(a => a.TelegramId).ToList();

            return Task.FromResult(result);
        }

        public Task<List<TgUserModel>> GetClientsTgIds(List<string> phones, bool uz, bool ru, bool en)
        {
            List<TgUserModel> result = new();
            using (var conn = new SqlConnection(connectionString))
            {
                var sb = new StringBuilder();
                sb.Append(@"SELECT [TelegramId],[Phone],[Language]
                          FROM [Users]
                          where IsDeleted=0 ");

                var phoneList = phones.Select(p => p.StartsWith("+") ? p : "+" + p).Distinct().ToList();
                sb.AppendLine($"AND Phone in ('{string.Join(",", phoneList).Replace(",", "','")}')");

                SqlCommand oCmd = new(sb.ToString(), conn);

                conn.Open();
                using (SqlDataReader oReader = oCmd.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        var user = new TgUserModel
                        {
                            TelegramId = oReader["TelegramId"] != DBNull.Value ? oReader["TelegramId"].ToString() : null,
                            Phone = oReader["Phone"] != DBNull.Value ? oReader["Phone"].ToString() : null,
                            Language = oReader["Language"] != DBNull.Value ? (int)oReader["Language"] : 0,
                        };
                        result.Add(user);
                    }

                    conn.Close();
                }
            }

            List<int> langs = new List<int>();
            if (uz)
                langs.Add(1);
            if (ru)
                langs.Add(2);
            if (en)
                langs.Add(3);
            if (langs.Count > 0)
            {
                result = (from tgPhone in result
                          join lang in langs on tgPhone.Language equals lang
                          select tgPhone).ToList();
            }

            result = result.DistinctBy(a => a.TelegramId).ToList();

            return Task.FromResult(result);
        }
    }
    public class TgUserModel
    {
        public string TelegramId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int Language { get; set; }
    }
}
