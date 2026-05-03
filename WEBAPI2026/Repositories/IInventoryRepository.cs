using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Repositories
{
    // IInventoryRepository 是「庫存資料 Repository 的介面」
    //
    // 用 React / Go 角度理解：
    // 這像是先定義資料來源的 contract。
    //
    // Service 只知道它可以呼叫 GetInventory，
    // 不需要知道資料是 mock data、Oracle、SQL Server 還是其他 DB。
    //
    // 現階段：
    // Repository 會回傳 mock data。
    //
    // 未來：
    // Repository 會改成使用 OracleConnection 查資料。
    public interface IInventoryRepository
    {
        // GetInventory 負責取得庫存資料
        //
        // request 裡面包含：
        // dateTimestampGTE
        // dateTimestampLTE
        //
        // 未來接 DB 時，會用這兩個欄位做查詢條件。
        List<InventoryDto> GetInventory(DateRangeRequest request);
    }
}