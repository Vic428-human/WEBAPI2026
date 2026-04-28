using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WEBAPI2026.Helpers; // 剛才新增：引用 AuthHeaderHelper，用來檢查 appid / timestamp / sign
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Models.Responses;

namespace WEBAPI2026.Controllers
{
    // [ApiController] 代表這是一個 Web API Controller
    //
    // 用 React / Next.js 角度理解：
    // 這個 class 就像一組 API route handler。
    //
    // 例如：
    // Next.js: app/api/so/route.ts
    // ASP.NET Core: SalesOrderController.cs
    [ApiController]

    // [Route("api/so")] 代表這支 API 的路徑是 /api/so
    //
    // 所以外部系統會這樣呼叫：
    // POST https://localhost:xxxx/api/so
    [Route("api/so")]

    // 告訴 Swagger / 外部使用者：
    // 這支 API 回傳格式是 application/json。
    //
    // 用 React 角度理解：
    // 這就像告訴前端：
    // response 的 Content-Type 是 application/json。
    [Produces("application/json")]
    public class SalesOrderController : ControllerBase
    {
        // [HttpPost] 代表這個 method 只接受 POST
        //
        // 文件要求所有 API 統一使用 POST，不使用 GET。
        //
        // 用 React 角度理解：
        // 你前端會這樣呼叫：
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
        //       sign: "test-sign"
        //     }
        //   }
        // );
        [HttpPost]
        public ActionResult<ApiResponse<SalesOrderDto>> GetSalesOrders(
            [FromBody] DateRangeRequest request,

            // 剛才修改：新增 [FromHeader(Name = "appid")] string appid
            //
            // 用 React 角度理解：
            // 這是從 request headers 裡面讀取 appid。
            //
            // 類似前端送：
            //
            // headers: {
            //   appid: "test-app"
            // }
            //
            // 後端這裡就可以用 appid 這個變數拿到它。
            [FromHeader(Name = "appid")] string appid,

            // 剛才修改：新增 [FromHeader(Name = "timestamp")] string timestamp
            //
            // 文檔要求 timestamp 是 13 位 millisecond UNIX timestamp。
            //
            // 用 React 角度理解：
            // 這也是從 request headers 讀取的值。
            [FromHeader(Name = "timestamp")] string timestamp,

            // 剛才修改：新增 [FromHeader(Name = "sign")] string sign
            //
            // sign 是外部系統根據 body + appid + timestamp + secretKey
            // 產生出來的簽名值。
            //
            // 目前這一階段只先檢查有沒有傳 sign。
            // 下一階段才會真的做 MD5 簽名比對。
            [FromHeader(Name = "sign")] string sign)
        {
            // 剛才新增：先檢查 Header 認證資料
            //
            // 用 React / Next.js route handler 角度理解：
            // 這就像 API route 一進來，先檢查 request.headers。
            //
            // 如果 appid / timestamp / sign 不完整，
            // 就直接 return 401，不繼續處理 request body。
            //
            // 目前這裡呼叫的是：
            // Helpers/AuthHeaderHelper.cs
            //
            // 它負責檢查：
            // 1. appid 是否存在
            // 2. timestamp 是否存在
            // 3. timestamp 是否為 13 位數字
            // 4. sign 是否存在
            if (!AuthHeaderHelper.TryValidateRequiredHeaders(appid, timestamp, sign, out string authErrorMessage))
            {
                return Unauthorized(new ApiResponse<SalesOrderDto>
                {
                    Message = authErrorMessage,
                    Status = 401,
                    Data = new List<SalesOrderDto>()
                });
            }

            // [FromBody] 的意思是：
            // 從 HTTP request body 讀 JSON，然後轉成 DateRangeRequest 物件。
            //
            // 用 React 角度理解：
            // 前端送出的 payload：
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
            // 所以如果沒有傳起始時間，我們直接回 400 Bad Request。
            if (request == null || string.IsNullOrWhiteSpace(request.DateTimestampGTE))
            {
                return BadRequest(new ApiResponse<SalesOrderDto>
                {
                    Message = "dateTimestampGTE is required",
                    Status = 400,
                    Data = new List<SalesOrderDto>()
                });
            }

            // 這裡先建立假資料。
            //
            // 目前第一階段目的不是接資料庫，
            // 而是先確認：
            //
            // 1. /api/so 可以被 Swagger 找到
            // 2. POST request body 可以被接住
            // 3. Header appid / timestamp / sign 可以被接住
            // 4. Response 格式符合文件
            //
            // 後面接 SQL Server 時，
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
                    // 看起來像拼字錯誤，但第一版先照文件。
                    TransationTS = "2026-04-27 10:30:00",

                    // 商品料號
                    MPNID = "MPN001",

                    // 商品序號
                    SerialNumber = "SN123456789",

                    // 交易類型
                    // 文件提到可能是 Sale / Return
                    TransactionType = "Sale",

                    // 資料更新時間
                    // 後面會用這個欄位做增量資料查詢
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
            // 前端收到 response 後會從 response.data.Data 取得真正資料陣列。
            return Ok(new ApiResponse<SalesOrderDto>
            {
                Message = "Success",
                Status = 200,
                Data = data
            });
        }
    }
}