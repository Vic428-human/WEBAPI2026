using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // 新增：用來讀取 appsettings.json 裡的 ApiAuth:SecretKey
using WEBAPI2026.Helpers; // 新增：使用 AuthHeaderHelper 做 Header + sign 驗證
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Models.Responses;
using WEBAPI2026.Services;

namespace WEBAPI2026.Controllers
{
    // [ApiController] 代表這是一個 Web API Controller
    //
    // 用 React / Next.js 角度理解：
    // 這個 class 就像 app/api/inventory/route.ts。
    //
    // 它負責處理 POST /api/inventory 這支 API。
    [ApiController]

    // [Route("api/inventory")] 代表這支 API 的路徑是 /api/inventory
    //
    // 外部系統會這樣呼叫：
    // POST https://localhost:xxxx/api/inventory
    [Route("api/inventory")]

    // 告訴 Swagger / 外部呼叫者：
    // 這支 API 回傳的是 application/json。
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        // 用來讀取 appsettings.json 裡面的設定
        // 這裡會讀： ApiAuth:SecretKey
        private readonly IConfiguration _configuration;
        private readonly InventoryService _inventoryService;

        // 透過 constructor injection 取得 IConfiguration 、 InventoryService
        // ASP.NET Core 會自動把 IConfiguration 、 InventoryService傳進來。
        public InventoryController(
            IConfiguration configuration,
            InventoryService inventoryService)
        {
            _configuration = configuration;
            _inventoryService = inventoryService;
        }

        // [HttpPost] 代表這個 method 只接受 POST
        //
        // 文件要求所有 API 統一使用 POST，不使用 GET。
        //
        // 用 React / axios 角度理解：
        //
        // await axios.post(
        //   "/api/inventory",
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
        public ActionResult<ApiResponse<InventoryDto>> GetInventory(
            [FromBody] DateRangeRequest request,

            // 新增：從 HTTP Header 讀取 appid
            //
            // 用 React 角度理解：
            // 對方送 headers.appid，
            // 後端這裡用 appid 變數接住。
            [FromHeader(Name = "appid")] string appid,

            // 新增：從 HTTP Header 讀取 timestamp
            //
            // 文件要求 timestamp 是 13 位 millisecond UNIX timestamp。
            [FromHeader(Name = "timestamp")] string timestamp,

            // 新增：從 HTTP Header 讀取 sign
            //
            // 這是對方根據：
            // request body + appid + timestamp + secretKey
            // 算出來的 MD5 簽名。
            [FromHeader(Name = "sign")] string sign)
        {
            // 新增：從 appsettings.json 讀取 secretKey
            //
            // appsettings.json 需要有：
            //
            // "ApiAuth": {
            //   "SecretKey": "test-secret-key"
            // }
            string secretKey = _configuration["ApiAuth:SecretKey"];

            // 新增：Inventory API 也要做正式 sign 驗證
            //
            // 這裡和 SalesOrderController.cs 的邏輯一致：
            //
            // 1. 檢查 appid 是否存在
            // 2. 檢查 timestamp 是否存在
            // 3. 檢查 timestamp 是否為 13 位數字
            // 4. 檢查 sign 是否存在
            // 5. 用 SignatureHelper 重新計算 expectedSign
            // 6. 比對 expectedSign 和對方 header 傳來的 sign
            //
            // 如果不通過，直接回 401。
            if (!AuthHeaderHelper.TryValidateRequiredHeadersAndSign(
                request,
                appid,
                timestamp,
                sign,
                secretKey,
                out string authErrorMessage))
            {
                return Unauthorized(new ApiResponse<InventoryDto>
                {
                    Message = authErrorMessage,
                    Status = 401,
                    Data = new List<InventoryDto>()
                });
            }

            // 修改：改用 DateRangeValidator 統一驗證 request body
            //
            // 原本这里只檢查 dateTimestampGTE 有沒有填。
            // 現在改成統一檢查：
            //
            // 1. request body 是否存在
            // 2. dateTimestampGTE 是否必填
            // 3. dateTimestampGTE 格式是否為 yyyy-MM-dd HH:mm:ss
            // 4. dateTimestampLTE 如果有填，格式是否為 yyyy-MM-dd HH:mm:ss
            // 5. dateTimestampGTE 是否晚於 dateTimestampLTE
            //
            // 用 React 工程師角度理解：
            // 這就像把 form validation 從 component 裡抽出去，
            // 改成共用 validateDateRangeForm(values)。
            if (!DateRangeValidator.TryValidate(request, out string dateRangeErrorMessage))
            {
                return BadRequest(new ApiResponse<SalesOrderDto>
                {
                    Message = dateRangeErrorMessage,
                    Status = 400,
                    Data = new List<SalesOrderDto>()
                });
            }

            // 修改：資料取得邏輯改交給 InventoryService
            //
            // 原本 Controller 自己建立 mock data。
            // 現在改成呼叫 Service。
            //
            // 用 Go 角度理解：
            // handler 不直接查資料，
            // 而是呼叫 service.GetInventory(...)。
            var data = _inventoryService.GetInventory(request);

            // 回傳成功格式
            //
            // 文件要求 response body 統一是：
            //
            // {
            //   "Message": "Success",
            //   "Status": 200,
            //   "Data": [...]
            // }
            return Ok(new ApiResponse<InventoryDto>
            {
                Message = "Success",
                Status = 200,
                Data = data
            });
        }
    }
}

/*
/api/inventory 執行之前，要先在 /api/debug/sign 確定有拿到 sign

Headers 填：
 appid: test-app
timestamp: 1538207443910
sign: 剛才 debug API 產生的 Sign
Content-Type: application/json
Accept: application/json

Body 必須和剛才產生 sign 時一致：
{
  "dateTimestampGTE": "2026-04-27 00:00:00",
  "dateTimestampLTE": "2026-04-27 23:59:59"
}

成功時預期會回：
{
  "Message": "Success",
  "Status": 200,
  "Data": [
    {
      "POSAppleID": "POS001",
      "Date": "2026-04-27",
      "MPNID": "MPN001",
      "Qty": 100,
      "UpdateTS": "2026-04-27 10:35:00"
    }
  ]
}
 */