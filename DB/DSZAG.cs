using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ZagAPIServer.DB.DSZAG.TransactionsToSyncWithBankingSystem;

namespace ZagAPIServer.DB.DSZAG
{
    internal static class DbConfig
    {
        // Read once; if you want lazy error handling you can wrap in Lazy<T>.
        internal static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["ZagTraderConnectionString"].ConnectionString
            ?? throw new ConfigurationErrorsException("Missing ZagTraderConnectionString.");
    }

    internal static class SqlLoader
    {
        private const int MaxRetryAttempts = 3;
        private const int BaseRetryDelayMilliseconds = 200;

        private static bool ShouldRetryOnDeadlock(SqlException ex)
        {
            if (ex == null)
            {
                return false;
            }

            foreach (SqlError error in ex.Errors)
            {
                if (error.Number == 1205) // Deadlock victim
                {
                    return true;
                }
            }

            return false;
        }

        private static TimeSpan GetRetryDelay(int attempt)
        {
            if (attempt <= 0)
            {
                attempt = 1;
            }

            int backoffFactor = 1 << (attempt - 1); // Exponential backoff: 1, 2, 4...
            int delay = BaseRetryDelayMilliseconds * backoffFactor;
            return TimeSpan.FromMilliseconds(delay);
        }

        /// <summary>
        /// Backward-compatible overload (no parameters).
        /// </summary>
        internal static Task<DataTable> LoadTableAsync(string sql, string resultTableName)
            => LoadTableAsync(sql, resultTableName, null, CommandType.Text, null);

        /// <summary>
        /// Enhanced overload supporting parameters, command type, and optional timeout.
        /// Pass an Action<SqlCommand> to add parameters / configure the command.
        /// </summary>
        internal static async Task<DataTable> LoadTableAsync(
            string sql,
            string resultTableName,
            Action<SqlCommand> configureCommand,
            CommandType commandType = CommandType.Text,
            int? timeoutSeconds = null)
        {
            if (string.IsNullOrWhiteSpace(DbConfig.ConnectionString))
                throw new ArgumentException("Connection string is required.", nameof(DbConfig.ConnectionString));
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL is required.", nameof(sql));
            if (string.IsNullOrWhiteSpace(resultTableName))
                throw new ArgumentException("Result table name is required.", nameof(resultTableName));

            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    var table = new DataTable(resultTableName);

                    using (var conn = new SqlConnection(DbConfig.ConnectionString))
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = commandType;
                        if (timeoutSeconds.HasValue && timeoutSeconds.Value > 0)
                            cmd.CommandTimeout = timeoutSeconds.Value;

                        configureCommand?.Invoke(cmd);

                        await conn.OpenAsync().ConfigureAwait(false);
                        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess).ConfigureAwait(false))
                        {
                            table.Load(reader);
                        }
                    }

                    return table;
                }
                catch (SqlException ex) when (ShouldRetryOnDeadlock(ex) && attempt < MaxRetryAttempts)
                {
                    await Task.Delay(GetRetryDelay(attempt)).ConfigureAwait(false);
                    continue;
                }
            }
        }
    }

    internal static class TransactionsToSyncWithBankingSystem
    {
        /// <summary>
        /// Asynchronous version if you prefer async I/O.
        /// </summary>
        internal static Task<DataTable> Fill()
        {
            const string sql = "SELECT * FROM TransactionsToSyncWithBankingSystem";
            return SqlLoader.LoadTableAsync(sql, "TransactionsToSyncWithBankingSystem");
        }
    }

    internal static class TransactionsSentBankingSystem
    {
        internal static Task<DataTable> Fill()
        {
            const string sql = "SELECT TransactionsSentBankingSystem.* FROM TransactionsSentBankingSystem";
            return SqlLoader.LoadTableAsync(sql, "TransactionsSentBankingSystem");
        }

        internal static async Task<int> InsertQuery(string TransactionID, DateTime TStamp, string BankRefNumber)
        {
            // Preserves original intent (insert + scalar). Returning Convert.ToInt32 may fail if TransactionID isn't numeric.
            // Left as-is to avoid changing business logic semantics.
            const string sql =
                "INSERT INTO [TransactionsSentBankingSystem] ([TransactionID], [TStamp], [BankRefNumber]) " +
                "VALUES (@TransactionID, @TStamp, @BankRefNumber); " +
                "SELECT TransactionID FROM TransactionsSentBankingSystem WHERE (TransactionID = @TransactionID);";

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = TransactionID;
                cmd.Parameters.Add("@TStamp", SqlDbType.DateTime).Value = TStamp;
                cmd.Parameters.Add("@BankRefNumber", SqlDbType.NVarChar).Value = (object)BankRefNumber ?? DBNull.Value;

                await conn.OpenAsync().ConfigureAwait(false);
                object scalar = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(scalar);
            }
        }

        internal static async Task<bool> ExistsAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            const string sql =
                "SELECT TOP (1) 1 FROM TransactionsSentBankingSystem WHERE TransactionID = @TransactionID;";

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar).Value = transactionId;

                await conn.OpenAsync().ConfigureAwait(false);
                object result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                return result != null && result != DBNull.Value;
            }
        }
    }

    internal static class BankIntegrationTransaction1
    {
        /// <summary>
        /// Retrieves a non-rejected bank integration transaction by TransactionID.
        /// Returns an empty DataTable if not found.
        /// </summary>
        internal static Task<DataTable> Fill(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            const string sql = @"
SELECT
    CreditBankAccountID,
    CreditAmount,
    BranchID,
    CurrencySymbol,
    TransactionID,
    GLDesc,
    TransactionDate,
    DebitBankAccountID,
    DebitAmount,
    BankReferenceNo,
    TStamp,
    CRReasonCode,
    DBReasonCode,
    CurrencyExchangeRateUSD,
    RateCode,
    CurrencySymbol2,
    RejectionNote,
    Rejected,
    ValueDate,
    ZAGEMP
FROM BankIntegrationTransaction
WHERE TransactionID = @Trans AND Rejected = 0;";

            return SqlLoader.LoadTableAsync(
                sql,
                "BankIntegrationTransaction",
                cmd =>
                {
                    var p = cmd.Parameters.Add("@Trans", SqlDbType.NVarChar);
                    p.Value = transactionId;
                });
        }

        internal static Task<DataTable> FillBy(string transactionId)
        {
            const string sql =
                "SELECT BankReferenceNo, BranchID, CRReasonCode, CreditAmount, CreditBankAccountID, CurrencyExchangeRateUSD, " +
                "CurrencySymbol, CurrencySymbol2, DBReasonCode, DebitAmount, DebitBankAccountID, GLDesc, RateCode, Rejected, " +
                "RejectionNote, TStamp, TransactionDate, TransactionID, ValueDate, ZAGEMP " +
                "FROM BankIntegrationTransaction WHERE (TransactionID = @Trans) AND (Rejected <> 0)";

            return SqlLoader.LoadTableAsync(
                sql,
                "BankIntegrationTransaction",
                cmd =>
                {
                    var p = cmd.Parameters.Add("@Trans", SqlDbType.NVarChar);
                    p.Value = transactionId;
                });
        }

        internal static int DeleteQuery(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            // Empty BankReferenceNo constraint preserved (empty string).
            const string sql =
                "DELETE FROM BankIntegrationTransaction " +
                "WHERE (TransactionID = @transID) AND (BankReferenceNo = '');";

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@transID", SqlDbType.NVarChar).Value = transactionId;
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates BankReferenceNo and timestamp for a transaction.
        /// Returns the number of affected rows.
        /// </summary>
        internal static int UpdateQuery(string bankReferenceNo, string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            const string sql = @"
UPDATE BankIntegrationTransaction
SET BankReferenceNo = @RefNo,
    TStamp = GETDATE()
WHERE TransactionID = @Trans;";

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@RefNo", SqlDbType.NVarChar).Value = (object)bankReferenceNo ?? DBNull.Value;
                cmd.Parameters.Add("@Trans", SqlDbType.NVarChar).Value = transactionId;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Marks a transaction as rejected with a note and updates timestamp.
        /// Returns the number of affected rows.
        /// </summary>
        internal static int UpdateRejection(string rejectionNote, string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            const string sql = @"
UPDATE BankIntegrationTransaction
SET Rejected = 1,
    RejectionNote = @Note,
    TStamp = GETDATE()
WHERE TransactionID = @Trans;";

            using (var conn = new SqlConnection(DbConfig.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Note", SqlDbType.NVarChar).Value = (object)rejectionNote ?? DBNull.Value;
                cmd.Parameters.Add("@Trans", SqlDbType.NVarChar).Value = transactionId;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
    }
}