using System.Linq;

namespace WEBAPI2026.Helpers
{
    // AuthHeaderHelper 是「共用的 Header 檢查工具」
    //
    // 用 React 角度理解：
    // 這很像你前端會寫一個共用 function：
    //
    // validateAuthHeaders(headers)
    //
    // 讓 SO API 和 Inventory API 都可以共用同一套檢查邏輯，
    // 不用每個 Controller 都重複寫 appid / timestamp / sign 檢查。
    public static class AuthHeaderHelper
    {
        // TryValidateRequiredHeaders 用來檢查必要 Header 是否存在
        //
        // 回傳 true：
        // 代表 header 基本格式通過，可以繼續執行 API。
        //
        // 回傳 false：
        // 代表 header 不符合要求，Controller 應該回傳 401 Unauthorized。
        public static bool TryValidateRequiredHeaders(
            string appid,
            string timestamp,
            string sign,
            out string errorMessage)
        {
            // 檢查 appid
            //
            // 用 React 角度理解：
            // 就像你檢查 request headers 裡有沒有 headers.appid。
            if (string.IsNullOrWhiteSpace(appid))
            {
                errorMessage = "Missing appid header";
                return false;
            }

            // 檢查 timestamp
            //
            // 文檔要求 timestamp 是 13 位 millisecond UNIX timestamp。
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                errorMessage = "Missing timestamp header";
                return false;
            }

            // timestamp 必須是 13 位數字
            //
            // 例如：
            // 1538207443910
            if (timestamp.Length != 13 || !timestamp.All(char.IsDigit))
            {
                errorMessage = "Invalid timestamp header";
                return false;
            }

            // 檢查 sign
            //
            // 這一版只先檢查有沒有傳。
            // 下一階段才會真的根據 body + appid + timestamp + secretKey 產生 MD5 來比對。
            if (string.IsNullOrWhiteSpace(sign))
            {
                errorMessage = "Missing sign header";
                return false;
            }

            // 如果三個 header 都有，timestamp 也是 13 位數字，
            // 就先視為基本檢查通過。
            errorMessage = string.Empty;
            return true;
        }
    }
}