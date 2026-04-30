using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // 新增：用來讀取 appsettings.json 裡的 ApiAuth:SecretKey
using WEBAPI2026.Helpers; // 使用 AuthHeaderHelper 做 Header + sign 驗證
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Models.Responses;

namespace WEBAPI2026.Controllers
{
    // [ApiController] 代表這是一個 Web API Controller
    //
    // 用 React / Next.js 角度理解：
    // 這個 class 就像一個 API route handler。
    //
    // 例如：
    // Next.js: app/api/so/route.ts
    // ASP.NET Core: Controllers/SalesOrderController.cs
    [ApiController]

    // [Route("api/so")] 代表這支 API 的路徑是 /api/so
    //
    // 外部系統會這樣呼叫：
    // POST https://localhost:xxxx/api/so
    [Route("api/so")]

    // 告訴 Swagger / 外部呼叫者：
    // 這支 API 回傳的是 application/json。
    [Produces("application/json")]
    public class SalesOrderController : ControllerBase
    {
        // 新增：用來讀取 appsettings.json 裡面的設定
        //
        // 用 React 角度理解：
        // 這有點像在後端讀取環境設定，例如：
        //
        // process.env.API_SECRET_KEY
        //
        // 只是目前 ASP.NET Core 是從 appsettings.json 讀：
        //
        // ApiAuth:SecretKey
        private readonly IConfiguration _configuration;

        // 新增：透過 constructor injection 取得 IConfiguration
        //
        // ASP.NET Core 會自動把 IConfiguration 傳進來。
        //
        // 用 React 角度理解：
        // 比較像框架自動把全域設定注入到這個 API handler 裡。
        public SalesOrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // [HttpPost] 代表這個 method 只接受 POST
        //
        // 文件要求所有 API 統一使用 POST，不使用 GET。
        //
        // 用 React / axios 角度理解：
        //
        // await axios.post(
        //   "/api/so",
        //   {
        //     dateTimestampGTE: "2026-04-27 00:00:00",
        //     dateTimestampLTE: "2026-04-27 23:59:59"
        //   },
        //   {
        //     headers: {
        //       appid: "test-app",
        //       timestamp: "1538207443910",
        //       sign: "正確的MD5簽名"
        //     }
        //   }
        // );
        [HttpPost]
        public ActionResult<ApiResponse<SalesOrderDto>> GetSalesOrders(
            [FromBody] DateRangeRequest request,

            // 從 HTTP Header 讀取 appid
            //
            // 用 React 角度理解：
            // 對方送：
            //
            // headers: {
            //   appid: "test-app"
            // }
            //
            // 後端這裡就會收到 appid 這個變數。
            [FromHeader(Name = "appid")] string appid,

            // 從 HTTP Header 讀取 timestamp
            //
            // 文件要求 timestamp 是 13 位 millisecond UNIX timestamp。
            //
            // 例如：
            // 1538207443910
            [FromHeader(Name = "timestamp")] string timestamp,

            // 從 HTTP Header 讀取 sign
            //
            // 這個 sign 是對方根據：
            // request body + appid + timestamp + secretKey
            //
            // 依照文件規則產生的 MD5 簽名。
            //
            // 後端稍後會用 SignatureHelper 重新算一次，
            // 看對方傳來的 sign 是否正確。
            [FromHeader(Name = "sign")] string sign)
        {
            // 新增：從 appsettings.json 讀取 secretKey
            //
            // appsettings.json 需要有：
            //
            // "ApiAuth": {
            //   "SecretKey": "test-secret-key"
            // }
            //
            // 用 React 角度理解：
            // 這類似 process.env.API_SECRET_KEY。
            string secretKey = _configuration["ApiAuth:SecretKey"];

            // 修改重點：
            // 這裡不再只是檢查 appid / timestamp / sign 有沒有傳。
            //
            // 原本是：
            // AuthHeaderHelper.TryValidateRequiredHeaders(...)
            //
            // 現在改成：
            // AuthHeaderHelper.TryValidateRequiredHeadersAndSign(...)
            //
            // 也就是：
            // 1. 檢查 appid 是否存在
            // 2. 檢查 timestamp 是否存在
            // 3. 檢查 timestamp 是否為 13 位數字
            // 4. 檢查 sign 是否存在
            // 5. 使用 secretKey 驗證 sign 是否真的正確
            //
            // 用 React / Next.js route handler 角度理解：
            // API 一進來，先做 auth guard。
            // 如果 header 或 sign 不正確，就直接 return 401。
            if (!AuthHeaderHelper.TryValidateRequiredHeadersAndSign(
                request,
                appid,
                timestamp,
                sign,
                secretKey,
                out string authErrorMessage))
            {
                return Unauthorized(new ApiResponse<SalesOrderDto>
                {
                    Message = authErrorMessage,
                    Status = 401,
                    Data = new List<SalesOrderDto>()
                });
            }

            // [FromBody] 的意思是：
            // 從 HTTP request body 讀 JSON，
            // 然後轉成 DateRangeRequest 物件。
            //
            // 用 React 角度理解：
            // 對方送出的 payload：
            //
            // {
            //   "dateTimestampGTE": "2026-04-27 00:00:00",
            //   "dateTimestampLTE": "2026-04-27 23:59:59"
            // }
            //
            // 會被 ASP.NET Core 轉成：
            //
            // request.DateTimestampGTE
            // request.DateTimestampLTE

            // 檢查必填欄位 dateTimestampGTE
            //
            // 文件規定：
            // dateTimestampGTE 是必填
            // dateTimestampLTE 是選填
            //
            // 如果沒有傳起始時間，回傳 400 Bad Request。
            if (!DateRangeValidator.TryValidate(request, out string dateRangeErrorMessage))
            {
                return BadRequest(new ApiResponse<SalesOrderDto>
                {
                    Message = dateRangeErrorMessage,
                    Status = 400,
                    Data = new List<SalesOrderDto>()
                });
            }

            // 目前先建立假資料。
            //
            // 現階段目的：
            // 1. 確認 /api/so 可以被 Swagger 找到
            // 2. 確認 request body 可以被接住
            // 3. 確認 header 可以被接住
            // 4. 確認 sign 驗證流程有接上
            // 5. 確認 response 格式符合文件
            //
            // 之後接 SQL Server 時，
            // 這段 data 會改成從 Repository 查回來的資料。
            var data = new List<SalesOrderDto>
            {
                new SalesOrderDto
                {
                    // 唯一交易識別碼
                    // 類似 React list render 時會用的 key / id
                    TransactionID = Guid.NewGuid().ToString(),

                    // POS Apple ID
                    POSAppleID = "POS001",

                    // 發票號碼
                    InvoiceNumber = "INV202604270001",

                    // 交易時間
                    //
                    // 注意：
                    // 文件欄位名稱是 TransationTS，
                    // 看起來像拼字錯誤，但目前先照文件。
                    TransationTS = "2026-04-27 10:30:00",

                    // 商品料號
                    MPNID = "MPN001",

                    // 商品序號
                    SerialNumber = "SN123456789",

                    // 交易類型
                    // 文件提到可能是 Sale / Return
                    TransactionType = "Sale",

                    // 資料更新時間
                    // 之後正式接資料庫時，會用這個欄位做增量資料查詢。
                    UpdateTS = "2026-04-27 10:35:00",

                    // 備註
                    Comments = ""
                }
            };

            // 回傳成功格式
            //
            // 文件要求 response body 統一是：
            //
            // {
            //   "Message": "Success",
            //   "Status": 200,
            //   "Data": [...]
            // }
            //
            // 用 React 角度理解：
            // 前端或外部系統真正要處理的資料會在：
            //
            // response.Data
            return Ok(new ApiResponse<SalesOrderDto>
            {
                Message = "Success",
                Status = 200,
                Data = data
            });
        }
    }
}


/*
 appid: test-app
timestamp: 1538207443910
sign:test-sign
 */