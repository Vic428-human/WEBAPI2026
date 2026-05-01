using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Repositories
{
    // ISalesOrderRepository 是「銷售資料 Repository 的介面」
    //
    // 用 React / Go 角度理解：
    // 這像是先定義資料來源的 contract。
    // Service 只知道它可以呼叫 GetSalesOrders，
    // 不需要知道資料是 mock data、Oracle、SQL Server 還是其他 DB。
    public interface ISalesOrderRepository
    {
        // 取得銷售資料
        //
        // 目前 Repository 會先回傳 mock data。
        // 未來接 Oracle DB 時，會用 request 裡的時間條件查資料。
        List<SalesOrderDto> GetSalesOrders(DateRangeRequest request);
    }
}