using System.Linq;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Helpers
{
    // AuthHeaderHelper 是「共用的 Header 認證檢查工具」
    //
    // 用 React / Next.js 角度理解：
    // 這很像你在 API route handler 裡抽出一個共用 function：
    //
    // validateAuthHeadersAndSign({
    //   payload,
    //   appid,
    //   timestamp,
    //   sign,
    //   secretKey
    // })
    //
    // 讓 SO API 和 Inventory API 都可以共用同一套認證檢查邏輯。
    public static class AuthHeaderHelper
    {
        // TryValidateRequiredHeadersAndSign 是目前主要使用的方法
        //
        // 這個方法會做兩層檢查：
        //
        // 第一層：檢查 Header 基本欄位是否存在
        // - appid
        // - timestamp
        // - sign
        //
        // 第二層：檢查 sign 是否正確
        // - 呼叫 SignatureHelper.ValidateSign(...)
        // - 後端自己重新計算 expectedSign
        // - 比對 expectedSign 和對方 header 傳來的 sign
        public static bool TryValidateRequiredHeadersAndSign(
            DateRangeRequest request,
            string appid,
            string timestamp,
            string sign,
            string secretKey,
            out string errorMessage)
        {
            // Step 1：先檢查 appid / timestamp / sign 是否有傳
            //
            // 用 React 角度理解：
            // 這就像先檢查 request.headers 裡面必要欄位是否存在。
            if (!TryValidateRequiredHeaders(appid, timestamp, sign, out errorMessage))
            {
                return false;
            }

            // Step 2：檢查 secretKey 是否有設定
            //
            // secretKey 不是外部傳進來的，
            // 而是後端自己從 appsettings.json 讀出來的。
            //
            // 如果後端沒有 secretKey，就沒辦法重新計算 expectedSign。
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                errorMessage = "Missing secretKey configuration";
                return false;
            }

            // Step 3：真正驗證 sign 是否正確
            //
            // 用 React 角度理解：
            //
            // const isValid = validateSign(
            //   payload,
            //   appid,
            //   timestamp,
            //   secretKey,
            //   receivedSign
            // );
            //
            // 這裡的 sign 是對方從 header 傳進來的 receivedSign。
            bool isValidSign = SignatureHelper.ValidateSign(
                request,
                appid,
                timestamp,
                secretKey,
                sign
            );

            if (!isValidSign)
            {
                errorMessage = "Invalid sign";
                return false;
            }

            // Header 欄位存在，而且 sign 驗證通過。
            errorMessage = string.Empty;
            return true;
        }

        // TryValidateRequiredHeaders 只負責第一層基本檢查
        //
        // 這個方法保留下來，是因為它的責任很單純：
        // 只檢查 appid / timestamp / sign 是否存在，以及 timestamp 格式是否正確。
        public static bool TryValidateRequiredHeaders(
            string appid,
            string timestamp,
            string sign,
            out string errorMessage)
        {
            // 檢查 appid
            //
            // appid 是請求方身份標識。
            if (string.IsNullOrWhiteSpace(appid))
            {
                errorMessage = "Missing appid header";
                return false;
            }

            // 檢查 timestamp 是否有傳
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                errorMessage = "Missing timestamp header";
                return false;
            }

            // 文檔要求 timestamp 使用 millisecond 格式，也就是 13 位數字。
            //
            // 例如：
            // 1538207443910
            if (timestamp.Length != 13 || !timestamp.All(char.IsDigit))
            {
                errorMessage = "Invalid timestamp header";
                return false;
            }

            // 檢查 sign 是否有傳
            if (string.IsNullOrWhiteSpace(sign))
            {
                errorMessage = "Missing sign header";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }
    }
}