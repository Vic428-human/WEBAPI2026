using System;
using System.Collections.Generic; // List<T> 屬於這個的 命名空間，如果不引用這個命名空間，編譯器就找不到 List<T>
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;


namespace WEBAPI2026.Repositories
{
    // SalesOrderRepository 是「銷售資料來源層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller 像 handler
    // Service 像 business logic
    // Repository 像專門負責拿資料的地方
    //
    // 目前這裡先回傳 mock data。
    //
    // 之後接 DB 時，
    // 這個檔案會改成負責 SQL 查詢，
    // 例如查 SQL Server / PostgreSQL。
    public class SalesOrderRepository : ISalesOrderRepository
    {
        // GetSalesOrders 目前先回傳假資料
        //
        // 未來接資料庫時，這裡會使用：
        //
        // request.DateTimestampGTE
        // request.DateTimestampLTE
        //
        // 去查指定時間範圍內的 SO 資料。
        public List<SalesOrderDto> GetSalesOrders(DateRangeRequest request)
        {
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
                    // 文檔欄位名稱是 TransationTS，
                    // 目前先照文檔拼法。
                    TransationTS = "2026-04-27 10:30:00",

                    // 商品料號
                    MPNID = "MPN001",

                    // 商品序號
                    SerialNumber = "SN123456789",

                    // 交易類型
                    // 例如 Sale / Return
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