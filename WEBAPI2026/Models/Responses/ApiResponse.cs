using System.Collections.Generic;
// 實驗
namespace WEBAPI2026.Models.Responses
{
    // ApiResponse<T> 是「所有 API 統一回傳格式」
    //
    // 用 React 角度理解：
    // 這就像你前端打 API 後，永遠預期 response.data 長這樣：
    //
    // {
    //   "Message": "Success",
    //   "Status": 200,
    //   "Data": [...]
    // }
    //
    // T 代表 Data 裡面的每一筆資料型別。
    //
    // 例如：
    // ApiResponse<SalesOrderDto>
    // 代表 Data 是 List<SalesOrderDto>
    //
    // ApiResponse<InventoryDto>
    // 代表 Data 是 List<InventoryDto>
    public class ApiResponse<T> // → 定義外層 Message / Status / Data
    {
        // Message：回傳訊息
        //
        // 成功時通常是：
        // "Success"
        //
        // 失敗時可以是：
        // "dateTimestampGTE is required"
        // "Invalid signature"
        //
        // 用 React 角度理解：
        // 這很像你前端拿來顯示 toast / error message 的文字。
        public string Message { get; set; }

        // Status：狀態碼
        //
        // 成功：200
        // 請求錯誤：400
        // 認證失敗：401
        // 伺服器錯誤：500
        //
        // 注意：
        // 這是 response body 裡面的 Status，
        // 不是 HTTP StatusCode 本身。
        //
        // 但我們後面 Controller 也會盡量讓 HTTP status 和這個欄位一致。
        public int Status { get; set; }

        // Data：真正的資料內容
        //
        // 用 React 角度理解：
        // 這就是你前端拿來 map render 的陣列。
        //
        // 例如：
        // response.Data.map(item => ...)
        public List<T> Data { get; set; }
    }
}