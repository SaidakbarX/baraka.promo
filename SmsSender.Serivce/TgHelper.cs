using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace SmsSender.Serivce
{
    public class TgHelper
    {
        readonly string connectionString;
        readonly ILogger<TgHelper> _logger;
        public TgHelper(IConfiguration configuration, ILogger<TgHelper> logger)
        {
            connectionString = configuration.GetConnectionString("TgBotConnection");
            _logger = logger;
        }

        public void SetUserIsDeleted(List<long> telegramIds)
        {
            if (telegramIds != null && telegramIds.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(@$"UPDATE [Users]
                        SET IsDeleted=1
                        where TelegramId IN ({string.Join(',', telegramIds)}) ");
                try
                {
                    using var conn = new SqlConnection(connectionString);
                    SqlCommand oCmd = new(sb.ToString(), conn);
                    conn.Open();
                    oCmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"TgHelper.SetUserIsDeleted cmd:{sb.ToString()}");
                }
            }
        }
    }
}
