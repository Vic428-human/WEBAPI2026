using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Services
{
    // InventoryService 是「Inventory API 的業務邏輯層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller 很像 route handler / handler
    // Service 則是把資料處理邏輯抽出去的 service layer
    //
    // 原本 InventoryController 裡面直接建立假資料，
    // 現在把這段搬到 Service。
    //
    // 之後接資料庫時，Controller 不需要大改，
    // 只要讓 Service 去呼叫 Repository 即可。
    public class InventoryService
    {
        // GetInventory 負責取得庫存資料
        //
        // 目前第一版仍然回傳 mock data。
        //
        // 之後會改成：
        //
        // InventoryService
        // → InventoryRepository
        // → SQL Server
        //
        // 用 React 角度理解：
        // 這有點像把 mock inventory list
        // 從 route handler 抽成一個 fetchInventory() function。
        public List<InventoryDto> GetInventory(DateRangeRequest request)
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