using System;
using System.Collections.Generic;
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

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Debug SQL：
                    //
                    // 這裡不查 SALES_ORDERS，
                    // 而是用 Oracle 內建 DUAL 表直接回傳一筆假資料。
                    //
                    // 如果 /api/so 可以看到 DEBUG_ROW，
                    // 代表：
                    // 1. OracleConnectionFactory 正常
                    // 2. OracleConnection 正常
                    // 3. Repository 有被呼叫
                    // 4. reader mapping 正常
                    //
                    // 如果這樣還是 Data: []，
                    // 就代表目前程式可能沒有跑到這份 Repository，
                    // 或 IIS Express 還在跑舊版本。
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
                        ORDER BY UPDATE_TS
                    ";

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