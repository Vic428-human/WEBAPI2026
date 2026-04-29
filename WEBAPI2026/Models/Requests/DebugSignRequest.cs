using System.Text.Json.Serialization;

namespace WEBAPI2026.Models.Requests
{
    // DebugSignRequest 是開發測試用的 request body
    //
    // 用 React 角度理解：
    // 這就像你寫一個測試工具頁面時，
    // 讓使用者輸入 payload + appid + timestamp，
    // 然後後端幫你算出 sign。
    public class DebugSignRequest
    {
        // 對應正式 API 的 dateTimestampGTE
        [JsonPropertyName("dateTimestampGTE")]
        public string DateTimestampGTE { get; set; }

        // 對應正式 API 的 dateTimestampLTE
        [JsonPropertyName("dateTimestampLTE")]
        public string DateTimestampLTE { get; set; }

        // 測試用 appid
        //
        // 之後你在 Swagger 測正式 API 時，
        // Header 裡也要填同一個 appid。
        [JsonPropertyName("appid")]
        public string Appid { get; set; }

        // 測試用 timestamp
        //
        // 之後你在 Swagger 測正式 API 時，
        // Header 裡也要填同一個 timestamp。
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}