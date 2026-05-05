using System;
using System.Collections.Generic;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using WEBAPI2026.Data;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Repositories
{
    // InventoryRepository 是「庫存資料來源層」
    //
    // 用 React / Go 角度理解：
    //
    // Controller 像 handler：
    // - 接 request
    // - 驗證 header
    // - 回 response
    //
    // Service 像 business logic：
    // - 決定要取得 Inventory 資料
    //
    // Repository 像真正拿資料的地方：
    // - 之前是回傳 mock data
    // - 現在改成用 OracleConnection 查 SYSTEM.INVENTORY_STOCKS 表
    public class InventoryRepository : IInventoryRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        // 透過 constructor injection 取得 OracleConnectionFactory
        //
        // 用 Go 角度理解：
        // 這像是 repository 透過共用的 DB factory 取得 connection。
        public InventoryRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // GetInventory 負責從 Oracle 查詢庫存資料
        //
        // request.DateTimestampGTE / request.DateTimestampLTE
        // 會被轉成 DateTime，作為 UPDATE_TS 的查詢條件。
        //
        // 對應 SQL：
        //
        // WHERE UPDATE_TS >= :dateTimestampGTE
        //   AND UPDATE_TS <= :dateTimestampLTE
        public List<InventoryDto> GetInventory(DateRangeRequest request)
        {
            var data = new List<InventoryDto>();

            // 將 request body 裡的 dateTimestampGTE 轉成 C# DateTime
            //
            // DateRangeValidator 已經在 Controller 層確認格式是：
            // yyyy-MM-dd HH:mm:ss
            var startTime = DateTime.ParseExact(
                request.DateTimestampGTE,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            // dateTimestampLTE 是選填。
            // 如果沒傳，就查到目前時間。
            DateTime endTime;

            if (string.IsNullOrWhiteSpace(request.DateTimestampLTE))
            {
                endTime = DateTime.Now;
            }
            else
            {
                endTime = DateTime.ParseExact(
                    request.DateTimestampLTE,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture
                );
            }

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // 正式測試版 SQL：
                    //
                    // 查 SYSTEM.INVENTORY_STOCKS，
                    // 並用 UPDATE_TS 對應文件中的查詢時間範圍。
                    //
                    // DB 欄位：
                    // POS_APPLE_ID
                    // STOCK_DATE
                    // MPN_ID
                    // QTY
                    // UPDATE_TS
                    //
                    // API DTO 欄位：
                    // POSAppleID
                    // Date
                    // MPNID
                    // Qty
                    // UpdateTS
                    command.CommandText = @"
                        SELECT
                            POS_APPLE_ID,
                            STOCK_DATE,
                            MPN_ID,
                            QTY,
                            UPDATE_TS
                        FROM SYSTEM.INVENTORY_STOCKS
                        WHERE UPDATE_TS >= :dateTimestampGTE
                          AND UPDATE_TS <= :dateTimestampLTE
                        ORDER BY UPDATE_TS
                    ";

                    // 讓 Oracle 根據參數名稱綁定。
                    command.BindByName = true;

                    // dateTimestampGTE 對應查詢起始時間
                    command.Parameters.Add(
                        new OracleParameter("dateTimestampGTE", OracleDbType.Date)
                        {
                            Value = startTime
                        }
                    );

                    // dateTimestampLTE 對應查詢結束時間
                    command.Parameters.Add(
                        new OracleParameter("dateTimestampLTE", OracleDbType.Date)
                        {
                            Value = endTime
                        }
                    );

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 把 Oracle row 轉成 API 文件要求的 InventoryDto
                            data.Add(new InventoryDto
                            {
                                POSAppleID = reader["POS_APPLE_ID"]?.ToString(),

                                // 文件中的 Date 格式是 yyyy-MM-dd
                                Date = FormatOracleDateOnly(reader["STOCK_DATE"]),

                                MPNID = reader["MPN_ID"]?.ToString(),

                                // QTY 是 Oracle NUMBER，這裡轉成 int
                                Qty = Convert.ToInt32(reader["QTY"]),

                                // 文件中的 UpdateTS 格式是 yyyy-MM-dd HH:mm:ss
                                UpdateTS = FormatOracleDateTime(reader["UPDATE_TS"])
                            });
                        }
                    }
                }
            }

            return data;
        }

        // 將 Oracle DATE 轉成 yyyy-MM-dd
        //
        // 用於 InventoryDto.Date
        private string FormatOracleDateOnly(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            var dateTime = Convert.ToDateTime(value);

            return dateTime.ToString("yyyy-MM-dd");
        }

        // 將 Oracle DATE 轉成 yyyy-MM-dd HH:mm:ss
        //
        // 用於 InventoryDto.UpdateTS
        private string FormatOracleDateTime(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            var dateTime = Convert.ToDateTime(value);

            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}