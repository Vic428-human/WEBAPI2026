using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Repositories
{
    // InventoryRepository 是「庫存資料來源層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller 像 handler
    // Service 像 business logic
    // Repository 像專門負責拿資料的地方
    //
    // 目前這裡先回傳 mock data。
    //
    // 之後接 Oracle DB 時，
    // 這個檔案會改成負責 Oracle SQL 查詢。
    public class InventoryRepository : IInventoryRepository
    {
        // GetInventory 目前先回傳假資料
        //
        // 未來接資料庫時，這裡會使用：
        //
        // request.DateTimestampGTE
        // request.DateTimestampLTE
        //
        // 去查指定時間範圍內的庫存資料。
        public List<InventoryDto> GetInventory(DateRangeRequest request)
        {
            var data = new List<InventoryDto>
            {
                new InventoryDto
                {
                    // POS Apple ID
                    // 代表是哪一個銷售點 / reseller 的庫存資料。
                    POSAppleID = "POS001",

                    // 庫存日期
                    //
                    // 文件要求格式：
                    // YYYY-MM-DD
                    Date = "2026-04-27",

                    // 商品料號
                    MPNID = "MPN001",

                    // 庫存數量
                    Qty = 100,

                    // 資料更新時間
                    //
                    // 之後正式接資料庫時，
                    // 會用這個欄位做增量資料查詢。
                    UpdateTS = "2026-04-27 10:35:00"
                }
            };

            return data;
        }
    }
}