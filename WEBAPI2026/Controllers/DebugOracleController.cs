using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WEBAPI2026.Data;

namespace WEBAPI2026.Controllers
{
    // DebugOracleController 是開發階段測試 Oracle 連線用
    //
    // 用 React / Go 角度理解：
    // 這像是一個 dev-only health check endpoint。
    //
    // 它不是正式業務 API。
    // 正式 API 還是：
    //
    // POST /api/so
    // POST /api/inventory
    [ApiController]
    [Route("api/debug/oracle")]
    [Produces("application/json")]
    public class DebugOracleController : ControllerBase
    {
        private readonly OracleConnectionFactory _connectionFactory;
        private readonly IWebHostEnvironment _environment;

        public DebugOracleController(
            OracleConnectionFactory connectionFactory,
            IWebHostEnvironment environment)
        {
            _connectionFactory = connectionFactory;
            _environment = environment;
        }

        // GET /api/debug/oracle/ping
        //
        // 用途：
        // 測試是否可以成功連到 Oracle。
        //
        // 這裡使用 Oracle 常見測試 SQL：
        //
        // SELECT 1 FROM DUAL
        //
        // DUAL 是 Oracle 內建的虛擬表，常用來測試查詢是否正常。
        [HttpGet("ping")]
        public ActionResult<object> Ping()
        {
            // 只允許 Development 環境使用
            //
            // 避免正式環境暴露 DB 測試 endpoint。
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            try
            {
                // 建立 OracleConnection
                //
                // 用 React / Go 角度理解：
                // 這裡像是拿到 db client。
                using (var connection = _connectionFactory.CreateConnection())
                {
                    // 真正打開 Oracle 連線
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        // Oracle 測試查詢
                        command.CommandText = "SELECT 1 FROM DUAL";

                        var result = command.ExecuteScalar();

                        return Ok(new
                        {
                            Message = "Oracle connection success",
                            Status = 200,
                            Result = result
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果連線失敗，會回傳錯誤訊息
                //
                // 常見原因：
                // 1. connection string 錯
                // 2. 帳號密碼錯
                // 3. host / port / service name 錯
                // 4. VPN / 防火牆 / 公司網路未連
                // 5. Oracle 套件未安裝成功
                return StatusCode(500, new
                {
                    Message = "Oracle connection failed",
                    Status = 500,
                    Error = ex.Message
                });
            }
        }
    }
}