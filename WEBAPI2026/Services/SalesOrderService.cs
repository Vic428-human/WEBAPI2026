using System;
using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;


namespace WEBAPI2026.Services
{
    // SalesOrderService 是「SO API 的業務邏輯層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller 很像 route handler / handler
    // Service 很像你把資料處理邏輯抽出去的 function / service layer
    //
    // 原本 SalesOrderController 裡面直接建立假資料，
    // 現在把這段搬到 Service。
    //
    // 之後接資料庫時，Controller 不需要大改，
    // 只要讓 Service 去呼叫 Repository 即可。
    public class SalesOrderService
    {
        // GetSalesOrders 負責取得 SO 銷售資料
        //
        // 目前第一版仍然回傳 mock data。
        // 之後會改成：
        //
        // SalesOrderService
        // → SalesOrderRepository
        // → SQL Server
        //
        // 用 React 角度理解：
        // 這有點像把 mock data 從 component / route handler
        // 抽成一個 fetchSalesOrders() function。
        public List<SalesOrderDto> GetSalesOrders(DateRangeRequest request)
        {
            // 目前先保留假資料。
            //
            // 注意：
            // request 目前還沒有真的拿來查資料，
            // 因為資料庫尚未接上。
            //
            // 之後接 DB 時，會使用：
            // request.DateTimestampGTE
            // request.DateTimestampLTE
            //
            // 作為查詢條件。
            var data = new List<SalesOrderDto>
            {
                new SalesOrderDto
                {
                    // 唯一交易識別碼
                    TransactionID = Guid.NewGuid().ToString(),

                    // POS Apple ID
                    POSAppleID = "POS001",

                    // 發票號碼
                    InvoiceNumber = "INV202604270001",

                    // 交易時間
                    //
                    // 注意：
                    // 文件欄位名稱是 TransationTS，
                    // 目前先照文件拼法。
                    TransationTS = "2026-04-27 10:30:00",

                    // 商品料號
                    MPNID = "MPN001",

                    // 商品序號
                    SerialNumber = "SN123456789",

                    // 交易類型：Sale / Return
                    TransactionType = "Sale",

                    // 資料更新時間
                    UpdateTS = "2026-04-27 10:35:00",

                    // 備註
                    Comments = ""
                }
            };

            return data;
        }
    }
}