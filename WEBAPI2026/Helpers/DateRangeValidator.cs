using System;
using System.Globalization;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Helpers
{
    // DateRangeValidator 是「共用的日期範圍驗證工具」
    //
    // 用 React 工程師角度理解：
    // 這很像你在前端會寫一個共用 validation function：
    //
    // validateDateRangeForm(values)
    //
    // 只是現在我們是在後端驗證外部系統送進來的 request body。
    //
    // 這樣 SalesOrderController 和 InventoryController
    // 就不用各自重複寫 dateTimestampGTE / dateTimestampLTE 的檢查邏輯。
    public static class DateRangeValidator
    {
        // 文檔要求的日期時間格式：
        //
        // yyyy-MM-dd HH:mm:ss
        //
        // 例如：
        // 2026-04-27 00:00:00
        private const string RequiredDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        // TryValidate 用來驗證 DateRangeRequest 是否符合文檔要求
        //
        // 回傳 true：
        // 代表 request body 的日期欄位通過驗證。
        //
        // 回傳 false：
        // 代表日期欄位不符合要求，Controller 應該回傳 400 BadRequest。
        public static bool TryValidate(DateRangeRequest request, out string errorMessage)
        {
            // Step 1：檢查 request body 是否存在
            //
            // 用 React 角度理解：
            // 就像先確認 form values 不是 null。
            if (request == null)
            {
                errorMessage = "Request body is required";
                return false;
            }

            // Step 2：檢查 dateTimestampGTE 是否有填
            //
            // 文檔要求：
            // dateTimestampGTE 是必填。
            //
            // 用 React 角度理解：
            // 這就像 startDate 是 required field。
            if (string.IsNullOrWhiteSpace(request.DateTimestampGTE))
            {
                errorMessage = "dateTimestampGTE is required";
                return false;
            }

            // Step 3：檢查 dateTimestampGTE 格式是否正確
            //
            // 文檔要求格式：
            // yyyy-MM-dd HH:mm:ss
            if (!IsValidDateTimeFormat(request.DateTimestampGTE))
            {
                errorMessage = "dateTimestampGTE format must be yyyy-MM-dd HH:mm:ss";
                return false;
            }

            // Step 4：檢查 dateTimestampLTE
            //
            // 文檔要求：
            // dateTimestampLTE 是選填。
            //
            // 所以如果它沒有傳或是空字串，目前允許通過。
            //
            // 用 React 角度理解：
            // endDate 可以不填。
            if (string.IsNullOrWhiteSpace(request.DateTimestampLTE))
            {
                errorMessage = string.Empty;
                return true;
            }

            // Step 5：如果 dateTimestampLTE 有填，就檢查格式
            if (!IsValidDateTimeFormat(request.DateTimestampLTE))
            {
                errorMessage = "dateTimestampLTE format must be yyyy-MM-dd HH:mm:ss";
                return false;
            }

            // Step 6：檢查起始時間是否晚於結束時間
            //
            // 文檔沒有明確寫這條，但從查詢時間範圍的業務邏輯來看，
            // 起始時間不應該晚於結束時間。
            //
            // 例如：
            // GTE = 2026-04-28 00:00:00
            // LTE = 2026-04-27 23:59:59
            //
            // 這種查詢條件不合理。
            var startTime = ParseDateTime(request.DateTimestampGTE);
            var endTime = ParseDateTime(request.DateTimestampLTE);

            if (startTime > endTime)
            {
                errorMessage = "dateTimestampGTE cannot be later than dateTimestampLTE";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        // IsValidDateTimeFormat 用來檢查日期字串是否符合文檔格式
        //
        // DateTime.TryParseExact 的意思是：
        // 必須完全符合指定格式才算通過。
        //
        // 例如：
        // 2026-04-27 00:00:00 ✅
        // 2026/04/27 00:00:00 ❌
        // 2026-04-27 ❌
        // string ❌
        private static bool IsValidDateTimeFormat(string value)
        {
            return DateTime.TryParseExact(
                value,
                RequiredDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }

        // ParseDateTime 用來把已經驗證過格式的字串轉成 DateTime
        //
        // 注意：
        // 這個 method 只會在 TryParseExact 已經通過後使用。
        private static DateTime ParseDateTime(string value)
        {
            return DateTime.ParseExact(
                value,
                RequiredDateTimeFormat,
                CultureInfo.InvariantCulture
            );
        }
    }
}