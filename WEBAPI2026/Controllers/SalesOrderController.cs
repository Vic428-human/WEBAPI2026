using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Models.Responses;

namespace WEBAPI2026.Controllers
{
    // 這是一支 Web API Controller
    [ApiController]

    // 外部系統呼叫的路徑是 POST /api/so
    [Route("api/so")]

    // 告訴 Swagger / 外部使用者：
    // 這支 API 回傳格式是 application/json，
    // 不要顯示成 text/plain。
    //
    // 用 React 角度理解：
    // 這就像告訴前端：
    // response 的 Content-Type 是 application/json。
    [Produces("application/json")]
    public class SalesOrderController : ControllerBase // → 實際建立 / 回傳這些資料
    {
        [HttpPost]
        public ActionResult<ApiResponse<SalesOrderDto>> GetSalesOrders([FromBody] DateRangeRequest request) // 接收 request body
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DateTimestampGTE)) //   確保 dateTimestampGTE 欄位不是空字串、不是 null、也不是只有空白。
            {
                return BadRequest(new ApiResponse<SalesOrderDto>
                {
                    Message = "dateTimestampGTE is required",
                    Status = 400,
                    Data = new List<SalesOrderDto>()
                });
            }

            // 如果 dateTimestampGTE 有值，程式就會建立一筆 SalesOrderDto 測試資料，並回傳
            var data = new List<SalesOrderDto>
            {
                new SalesOrderDto
                {
                    TransactionID = Guid.NewGuid().ToString(),
                    POSAppleID = "POS001",
                    InvoiceNumber = "INV202604270001",
                    TransationTS = "2026-04-27 10:30:00",
                    MPNID = "MPN001",
                    SerialNumber = "SN123456789",
                    TransactionType = "Sale",
                    UpdateTS = "2026-04-27 10:35:00",
                    Comments = ""
                }
            };

            return Ok(new ApiResponse<SalesOrderDto>
            {
                Message = "Success",
                Status = 200,
                Data = data
            });
        }
    }
}