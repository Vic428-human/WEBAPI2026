using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WEBAPI2026.Helpers;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Controllers
{
    // DebugController 是「開發階段測試工具」
    //
    // 用 React 角度理解：
    // 這很像你在 dev 環境做一個小工具頁面，
    // 專門幫你產生測試用的 sign。
    //
    // 注意：
    // 這支 API 不應該提供給正式環境使用。
    [ApiController]
    [Route("api/debug")]
    [Produces("application/json")]
    public class DebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DebugController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // POST /api/debug/sign
        //
        // 用途：
        // 根據 request body + appid + timestamp + secretKey
        // 產生一組正確的 MD5 sign。
        //
        // 用 React 角度理解：
        // 這像是 dev tool：
        //
        // const sign = generateSign({
        //   dateTimestampGTE,
        //   dateTimestampLTE,
        //   appid,
        //   timestamp
        // });
        //
        // 然後你把這個 sign 拿去測正式 API。
        [HttpPost("sign")]
        public ActionResult<object> GenerateSign([FromBody] DebugSignRequest request)
        {
            // 只允許 Development 環境使用
            //
            // 如果不是 Development，就直接拒絕。
            // 避免正式環境暴露產生 sign 的工具。
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            // 從 appsettings.json 讀取 secretKey
            string secretKey = _configuration["ApiAuth:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return BadRequest(new
                {
                    Message = "Missing ApiAuth:SecretKey configuration",
                    Status = 400
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    Message = "Request body is required",
                    Status = 400
                });
            }

            // 把 DebugSignRequest 轉成正式 API 使用的 DateRangeRequest。
            //
            // 因為 SignatureHelper.GenerateSign 目前吃的是 DateRangeRequest。
            var dateRangeRequest = new DateRangeRequest
            {
                DateTimestampGTE = request.DateTimestampGTE,
                DateTimestampLTE = request.DateTimestampLTE
            };

            // 呼叫我們剛剛做好的 SignatureHelper.GenerateSign
            //
            // 這裡產生出來的 sign，
            // 就是你等等要放到 Swagger Header 裡的 sign。
            string sign = SignatureHelper.GenerateSign(
                dateRangeRequest,
                request.Appid,
                request.Timestamp,
                secretKey
            );

            // 回傳測試資訊
            //
            // 你之後測 /api/so 或 /api/inventory 時，
            // Header 要填：
            //
            // appid: response.Appid
            // timestamp: response.Timestamp
            // sign: response.Sign
            return Ok(new
            {
                Message = "Success",
                Status = 200,
                Appid = request.Appid,
                Timestamp = request.Timestamp,
                Sign = sign
            });
        }
    }
}

/*
 {
  "dateTimestampGTE": "2026-04-27 00:00:00",
  "dateTimestampLTE": "2026-04-27 23:59:59",
  "appid": "test-app",
  "timestamp": "1538207443910"
}

{
  "Message": "Success",
  "Status": 200,
  "Appid": "test-app",
  "Timestamp": "1538207443910",
  "Sign": "一串MD5字串"
}
 */