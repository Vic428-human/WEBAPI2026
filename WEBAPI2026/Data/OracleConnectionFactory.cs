using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace WEBAPI2026.Data
{
    // OracleConnectionFactory 是「建立 Oracle 連線的工廠」
    //
    // 用 React / Go 角度理解：
    // 這有點像你以前會集中管理 db connection，
    // 而不是在每個 handler / controller 裡重複寫連線設定。
    //
    // 這個 class 的責任：
    // 1. 從 appsettings.json 讀取 Oracle 連線字串
    // 2. 建立 OracleConnection
    // 3. 讓 Repository 之後可以共用這個連線建立方式
    public class OracleConnectionFactory
    {
        private readonly IConfiguration _configuration;

        // ASP.NET Core 會自動把 IConfiguration 注入進來
        //
        // 用 React 角度理解：
        // 類似你從 env/config 讀取設定值。
        public OracleConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 建立 OracleConnection
        //
        // 注意：
        // 這裡只是建立 connection object，
        // 還沒有真正連線。
        //
        // 真正連線會在外部呼叫：
        // connection.Open();
        public OracleConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("OracleDb");

            return new OracleConnection(connectionString);
        }
    }
}