using System;
using System.Collections.Generic;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
using WEBAPI2026.Data;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;

namespace WEBAPI2026.Repositories
{
    // SalesOrderRepository 是「銷售資料來源層」
    //
    // Debug 版本：
    // 這一版先不查 SYSTEM.SALES_ORDERS。
    // 先用 Oracle 內建的 DUAL 表回傳一筆固定資料。
    //
    // 用途：
    // 確認 Web API 是否真的有走到 OracleConnection 和 reader mapping。
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly OracleConnectionFactory _connectionFactory;

        public SalesOrderRepository(OracleConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<SalesOrderDto> GetSalesOrders(DateRangeRequest request)
        {
            var data = new List<SalesOrderDto>();

            // 將 request body 裡的 dateTimestampGTE 轉成 C# DateTime
            //
            // 用 React / JS 角度理解：
            // 這有點像把字串 "2026-04-27 00:00:00"
            // 轉成可以拿來比較時間的 Date object。
            var startTime = DateTime.ParseExact(
                request.DateTimestampGTE,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture
            );

            // dateTimestampLTE 是選填。
            // 如果有傳，就用它當查詢結束時間。
            // 如果沒傳，就用目前時間 DateTime.Now。
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
                    // 這裡改回查 SYSTEM.SALES_ORDERS，
                    // 並且依照文件 request body 的時間範圍查詢資料。
                    //
                    // dateTimestampGTE / dateTimestampLTE
                    // 會對應到 DB 裡的 UPDATE_TS。
                    command.CommandText = @"
        SELECT
            TRANSACTION_ID,
            POS_APPLE_ID,
            INVOICE_NUMBER,
            TRANSATION_TS,
            MPN_ID,
            SERIAL_NUMBER,
            TRANSACTION_TYPE,
            UPDATE_TS,
            COMMENTS
        FROM SYSTEM.SALES_ORDERS
        WHERE UPDATE_TS >= :dateTimestampGTE
          AND UPDATE_TS <= :dateTimestampLTE
        ORDER BY UPDATE_TS
    ";

                    // 讓 Oracle 根據參數名稱綁定，
                    // 而不是根據參數加入順序。
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
                            data.Add(new SalesOrderDto
                            {
                                TransactionID = reader["TRANSACTION_ID"]?.ToString(),
                                POSAppleID = reader["POS_APPLE_ID"]?.ToString(),
                                InvoiceNumber = reader["INVOICE_NUMBER"]?.ToString(),
                                TransationTS = FormatOracleDate(reader["TRANSATION_TS"]),
                                MPNID = reader["MPN_ID"]?.ToString(),
                                SerialNumber = reader["SERIAL_NUMBER"]?.ToString(),
                                TransactionType = reader["TRANSACTION_TYPE"]?.ToString(),
                                UpdateTS = FormatOracleDate(reader["UPDATE_TS"]),
                                Comments = reader["COMMENTS"]?.ToString()
                            });
                        }
                    }
                }
            }

            return data;
        }

        private string FormatOracleDate(object value)
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