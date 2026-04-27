using System.Text.Json.Serialization;

namespace WEBAPI2026.Models.Requests
{
    // DateRangeRequest 是「外部系統呼叫 API 時送進來的 request body 格式」
    //
    // 用 React 角度理解：
    // 這就像你在前端呼叫 API 時送出的 payload type：
    //
    // await axios.post("/api/so", {
    //   dateTimestampGTE: "2022-06-30 00:00:00",
    //   dateTimestampLTE: "2022-07-01 23:59:59"
    // });
    //
    // ASP.NET Core 會把 JSON body 自動轉成這個 C# class。
    public class DateRangeRequest
    {
        // 對應 JSON 裡的 dateTimestampGTE
        //
        // 用 React 角度理解：
        // 前端送的是 payload.dateTimestampGTE
        // 後端 C# 裡讀的是 request.DateTimestampGTE
        //
        // 這是必填欄位：
        // 代表查詢資料的起始時間。
        //
        // 文件要求格式：
        // yyyy-MM-dd HH:mm:ss
        //
        // 範例：
        // "2022-06-30 00:00:00"
        [JsonPropertyName("dateTimestampGTE")]
        public string DateTimestampGTE { get; set; }

        // 對應 JSON 裡的 dateTimestampLTE
        //
        // 這是選填欄位：
        // 代表查詢資料的結束時間。
        //
        // 用 React 角度理解：
        // 這就像 filter form 裡面的 endDate 可以不填。
        //
        // 如果外部系統沒有傳 dateTimestampLTE，
        // API 後面應該查詢：
        // 從 dateTimestampGTE 到現在的所有資料。
        //
        // 這裡用 string? 表示它可以是 null。
        [JsonPropertyName("dateTimestampLTE")]
        public string DateTimestampLTE { get; set; }
    }
}