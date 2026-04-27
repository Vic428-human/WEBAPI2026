namespace WEBAPI2026.Models.Dtos
{
    // InventoryDto 是「Inventory API 回傳給外部系統的每一筆庫存資料格式」
    //
    // 用 React 角度理解：
    // 這就像庫存列表 table 的每一列資料。
    //
    // inventoryList.map(item => (
    //   <tr>
    //     <td>{item.POSAppleID}</td>
    //     <td>{item.MPNID}</td>
    //     <td>{item.Qty}</td>
    //   </tr>
    // ))
    public class InventoryDto
    {
        // POS Apple ID
        // 代表是哪一個銷售點 / reseller 的庫存
        public string POSAppleID { get; set; }

        // 庫存日期
        //
        // 文件要求格式：
        // YYYY-MM-DD
        public string Date { get; set; }

        // 商品料號
        public string MPNID { get; set; }

        // 庫存數量
        public int Qty { get; set; }

        // 資料更新時間
        //
        // 後面查資料時會用這個欄位判斷資料是否有更新。
        public string UpdateTS { get; set; }
    }
}