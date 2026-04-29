using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Helpers
{
    // SignatureHelper 是「產生與驗證 sign 的工具」
    //
    // 用 React / 前端工程師角度理解：
    // 這很像你寫一個共用工具 function：
    //
    // generateSign(payload, appid, timestamp, secretKey)
    //
    // 目的不是處理 API response，
    // 而是根據 request body + headers + secretKey 算出一組簽名，
    // 再拿它跟 header 裡的 sign 比對。
    public static class SignatureHelper
    {
        // GenerateSign 用來根據文檔規則產生 MD5 sign
        //
        // 文檔規則簡化理解：
        // 1. 取出 body 裡的參數
        // 2. 按照 key 的字母順序排序
        // 3. 拼接成 key=value
        // 4. 再加上 appid、timestamp、secretKey
        // 5. 對整段字串做 MD5
        //
        // 用 React 角度理解：
        // request 就像前端送出的 payload：
        //
        // {
        //   dateTimestampGTE: "2026-04-27 00:00:00",
        //   dateTimestampLTE: "2026-04-27 23:59:59"
        // }
        //
        // appid / timestamp 則像 axios headers 裡的資料。
        public static string GenerateSign(
            DateRangeRequest request,
            string appid,
            string timestamp,
            string secretKey)
        {
            // 先建立一個排序用的 Dictionary。
            //
            // SortedDictionary 會自動根據 key 排序。
            //
            // 用 React 角度理解：
            // 類似把 object keys 做 sort：
            //
            // Object.keys(payload).sort()
            // dateTimestampGTE=2026-04-27 00:00:00dateTimestampLTE=2026-04-27 23:59:59appid=...timestamp=...secretKey=...
            var sortedParams = new SortedDictionary<string, string>();

            // dateTimestampGTE 是必填。
            // 如果有值，就加入簽名參數。
            if (request != null && request.DateTimestampGTE != null)
            {
                sortedParams.Add("dateTimestampGTE", request.DateTimestampGTE);
            }

            // dateTimestampLTE 是選填。
            //
            // 文檔說 null 值不參與拼接，
            // 所以只有不是 null 時才加入。
            if (request != null && request.DateTimestampLTE != null)
            {
                sortedParams.Add("dateTimestampLTE", request.DateTimestampLTE);
            }

            // 建立簽名前的原始字串。
            //
            // 例如可能會變成：
            //
            // rawSignString = dateTimestampGTE = 2026 - 04 - 27 00:00:00dateTimestampLTE = 2026 - 04 - 27 23:59:59appid = test - apptimestamp = 1538207443910secretKey = test - secret - key
            var rawSignString = BuildRawSignString(sortedParams, appid, timestamp, secretKey);

            // 對原始字串做 MD5，得到 sign。 d41d8cd98f00b204e9800998ecf8427e  
            return ToMd5(rawSignString);
        }

        // ValidateSign 用來比對外部傳進來的 sign 是否正確
        //
        // 用 React 角度理解：
        // 這像是：
        //
        // const expectedSign = generateSign(payload, appid, timestamp, secretKey);
        // return expectedSign === headerSign;
        //
        // 目前會忽略大小寫比對，避免對方傳大寫 MD5 時直接失敗。
        public static bool ValidateSign(
            DateRangeRequest request,
            string appid,
            string timestamp,
            string secretKey,
            string receivedSign)
        {
            // 如果 header 沒有 sign，直接失敗。
            if (string.IsNullOrWhiteSpace(receivedSign))
            {
                return false;
            }

            // 根據同一套規則產生後端預期的 sign。
            var expectedSign = GenerateSign(request, appid, timestamp, secretKey);

            // 比對外部傳進來的 sign 和後端算出來的 sign。
            //
            // StringComparison.OrdinalIgnoreCase：
            // 代表大小寫不同也視為相同。
            return string.Equals(expectedSign, receivedSign, StringComparison.OrdinalIgnoreCase);
        }

        // BuildRawSignString 負責組出 MD5 前的原始字串
        //
        // 這個 method 拆出來是為了方便 debug。
        //
        // 之後如果你想確認 sign 為什麼算錯，
        // 可以暫時把這個 raw string 印出來比對。
        private static string BuildRawSignString(
            SortedDictionary<string, string> sortedParams,
            string appid,
            string timestamp,
            string secretKey)
        {
            var builder = new StringBuilder();

            // 按 key 字典序，把 body 參數拼接成 key=value。
            foreach (var item in sortedParams)
            {
                builder.Append(item.Key);
                builder.Append("=");
                builder.Append(item.Value);
            }

            // 最後依照文檔要求，拼接 appid / timestamp / secretKey。
            builder.Append("appid=");
            builder.Append(appid);

            builder.Append("timestamp=");
            builder.Append(timestamp);

            builder.Append("secretKey=");
            builder.Append(secretKey);

            return builder.ToString();
        }

        // ToMd5 負責把字串轉成 MD5 hash
        //
        // 用 React / JS 角度理解：
        // 類似 crypto library 裡的：
        //
        // md5(rawString)
        //
        // 回傳結果會是小寫 32 位 hex 字串。
        private static string ToMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var builder = new StringBuilder();

                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}