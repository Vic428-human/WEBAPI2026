namespace WEBAPI2026.Models.Dtos
{
    // SalesOrderDto 是「SO API 回傳給外部系統的每一筆銷售資料格式」
    //
    // 用 React 角度理解：
    // 這很像你前端 table 裡面每一列 row data 的型別。
    //
    // 例如：
    // salesOrders.map(order => (
    //   <tr key={order.TransactionID}>
    //     <td>{order.InvoiceNumber}</td>
    //     <td>{order.SerialNumber}</td>
    //   </tr>
    // ))
    //
    // 這個 class 的欄位名稱要盡量跟 API 文件一致，
    // 因為外部系統會依照這些欄位名稱接資料。
    public class SalesOrderDto
    {
        // 唯一交易識別碼，類似資料的 primary key / id
        public string TransactionID { get; set; }

        // POS Apple ID
        // 代表銷售點 / reseller 對應的 Apple ID
        public string POSAppleID { get; set; }

        // 發票號碼
        public string InvoiceNumber { get; set; }

        // 交易時間
        //
        // 注意：
        // 文件裡寫的是 TransationTS，
        // 看起來像少了一個 c，
        // 但第一版先照文件欄位名稱，不要自行改成 TransactionTS。
        public string TransationTS { get; set; }

        // 商品料號
        public string MPNID { get; set; }

        // 商品序號
        public string SerialNumber { get; set; }

        // 交易類型
        //
        // 文件提到可能是：
        // Sale / Return
        public string TransactionType { get; set; }

        // 資料更新時間
        //
        // 後面查資料時會用這個時間判斷要拉哪些更新資料。
        public string UpdateTS { get; set; }

        // 備註
        public string Comments { get; set; }
    }
}