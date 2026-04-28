using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Models.Responses;

namespace WEBAPI2026.Controllers
{
    // [ApiController] 代表這是一個 Web API Controller
    //
    // 用 React / Next.js 角度理解：
    // 這個 class 很像一個 API route handler。
    //
    // 例如：
    // Next.js: app/api/inventory/route.ts
    // ASP.NET Core: Controllers/InventoryController.cs
    [ApiController]

    // [Route("api/inventory")] 代表這支 API 的 URL 是 /api/inventory
    //
    // 外部系統或 Swagger 會這樣呼叫：
    // POST https://localhost:xxxx/api/inventory
    [Route("api/inventory")]

    // 告訴 Swagger / 外部呼叫者：
    // 這支 API 回傳的是 JSON。
    //
    // 用 React 角度理解：
    // 這就像 fetch / axios 收到的 response content-type 是 application/json。
    [Produces("application/json")]
    public class InventoryController : ControllerBase
    {
        // [HttpPost] 代表這個 method 只接受 POST
        //
        // 文檔要求所有 API 統一使用 POST，不使用 GET。
        //
        // 用 React 角度理解：
        // 前端或外部系統會這樣呼叫：
        //
        // await axios.post("/api/inventory", {
        //   dateTimestampGTE: "2026-04-27 00:00:00",
        //   dateTimestampLTE: "2026-04-27 23:59:59"
        // });
        [HttpPost]
        public ActionResult<ApiResponse<InventoryDto>> GetInventory([FromBody] DateRangeRequest request)
        {
            // [FromBody] 的意思是：
            // 從 HTTP request body 讀取 JSON，
            // 然後自動轉成 DateRangeRequest 物件。
            //
            // 用 React 角度理解：
            // 外部送進來的 payload：
            //
            // {
            //   "dateTimestampGTE": "2026-04-27 00:00:00",
            //   "dateTimestampLTE": "2026-04-27 23:59:59"
            // }
            //
            // 會變成後端 C# 裡的：
            //
            // request.DateTimestampGTE
            // request.DateTimestampLTE

            // 檢查必填欄位 dateTimestampGTE
            //
            // 文檔規定：
            // dateTimestampGTE 是必填
            // dateTimestampLTE 是選填
            //
            // 所以如果沒有起始時間，就回傳 400 BadRequest。
            if (request == null || string.IsNullOrWhiteSpace(request.DateTimestampGTE))
            {
                return BadRequest(new ApiResponse<InventoryDto>
                {
                    Message = "dateTimestampGTE is required",
                    Status = 400,
                    Data = new List<InventoryDto>()
                });
            }

            // 這裡先建立假資料。
            //
            // 目前第一階段目的不是接資料庫，
            // 而是先確認：
            //
            // 1. /api/inventory 可以被 Swagger 找到
            // 2. Request body 格式正確
            // 3. Response body 格式符合文檔
            // 4. Inventory 的 Data 欄位符合文檔要求
            //
            // 後面接 SQL Server 時，
            // 這段 data 會改成從 Repository 查回來的資料。
            var data = new List<InventoryDto>
            {
                new InventoryDto
                {
                    // POS Apple ID
                    // 代表是哪一個銷售點 / reseller 的庫存資料
                    POSAppleID = "POS001",

                    // 庫存日期
                    // 文檔要求格式是 YYYY-MM-DD
                    Date = "2026-04-27",

                    // 商品料號
                    MPNID = "MPN001",

                    // 庫存數量
                    Qty = 100,

                    // 資料更新時間
                    // 後面正式查資料時，會用這個時間做增量查詢
                    UpdateTS = "2026-04-27 10:35:00"
                }
            };

            // 回傳成功格式
            //
            // 文檔要求 response body 統一是：
            //
            // {
            //   "Message": "Success",
            //   "Status": 200,
            //   "Data": [...]
            // }
            //
            // 用 React 角度理解：
            // 前端收到 response 後，
            // 真正要 render 的資料會在 response.data.Data 裡面。
            return Ok(new ApiResponse<InventoryDto>
            {
                Message = "Success",
                Status = 200,
                Data = data
            });
        }
    }
}


// 這一步是在新增 Inventory API 的入口 POST /api/inventory，讓外部系統可以依照文件格式，用時間範圍「查詢庫存」資料並收到統一格式的 JSON 回應。

/*
 文檔裡有明確寫到需要做庫存 API，位置在：
6.2 獲取庫存表單數據（Inventory API）
6.2.1 業務說明：取得每日庫存資料

文檔不是只要求 SO 銷售資料，還另外要求一支 Inventory API，用途是「取得每日庫存資料」
 */

/* 用這個 body 測試：
 {
  "dateTimestampGTE": "2026-04-27 00:00:00",
  "dateTimestampLTE": "2026-04-27 23:59:59"
}
 */

/* 預期 response 會類似:
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