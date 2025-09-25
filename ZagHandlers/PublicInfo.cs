// VBConversions Note: VB project level imports
using IBM.WMQ.Nmqi;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
// End of VB project level imports

using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ZagAPIServer;
using ZagAPIServer.DB.DSZAG;
using ZagAPIServer.Tools;


[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace ZagAPIServer
{
    sealed class PublicInfo
    {
        public static Dictionary<string, Type> AllhandlerColl = new Dictionary<string, Type>(); //BindingList(Of HandlersItems)
        public static Dictionary<string, int> TransactionsHs = new Dictionary<string, int>();
        public static Dictionary<string, byte[]> MesgIDs = new Dictionary<string, byte[]>();
        public static Dictionary<string, short> IncremntalIDs = new Dictionary<string, short>();
        private static HashSet<string> processedTransactionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, bool> sentStatusCache = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);


        public static Logger log = new Logger();

        public static void RegisterAllHandlers()
        {
            AllhandlerColl.Clear();

            AllhandlerColl.Add("Housing Bank IBM Websphare MQ", typeof(HousingBankCoreBankHandler));
            AllhandlerColl.Add("Housing Bank Simulator", typeof(HousingBankCoreBankSimulatorHandler));



        }

        static System.Threading.Timer Fivesec = null;

        public static void runtimer()
        {
            Fivesec = new System.Threading.Timer((state) =>
            {
                GetTransactionsToSyncFromZagDB();
                InTimer = false;

                Fivesec.Change(1000, System.Threading.Timeout.Infinite);
            }, null, 0, System.Threading.Timeout.Infinite);


        }


        static bool InTimer = false;

        public static async void GetTransactionsToSyncFromZagDB()
        {
            // Exit Sub

            if (InTimer == true)
            {
                return;
            }
            InTimer = true;
            try
            {
                DataTable xDTTransactionsToSyncWithBankingSystem = TransactionsToSyncWithBankingSystem.Fill().GetAwaiter().GetResult();

                foreach (DataRow row in xDTTransactionsToSyncWithBankingSystem.Rows)
                {
                    string TransactionID = row["TransactionID"].ToString();
                    if (string.IsNullOrWhiteSpace(TransactionID))
                    {
                        log.LogTransaction.Warn("Encountered transaction row with empty TransactionID; skipping.");
                        continue;
                    }

                    if (!processedTransactionIds.Add(TransactionID))
                    {
                        log.LogTransaction.Debug($"Skipping duplicate row for TransactionID : {TransactionID} in current batch.");
                        continue;
                    }

                    bool alreadySent;
                    if (!sentStatusCache.TryGetValue(TransactionID, out alreadySent))
                    {
                        alreadySent = await TransactionsSentBankingSystem.ExistsAsync(TransactionID).ConfigureAwait(false);
                        sentStatusCache[TransactionID] = alreadySent;
                    }

                    if (alreadySent)
                    {
                        log.LogTransaction.Info($"TransactionID : {TransactionID} already recorded as sent. Skipping duplicate request.");
                        continue;
                    }
                    //If TransactionID = "2017-12-20-111" Then
                    try
                    {
                        DataTable xDTBankIntegrationTransaction1 = BankIntegrationTransaction1.FillBy(TransactionID).GetAwaiter().GetResult();
                        if (xDTBankIntegrationTransaction1.Rows.Count > 0)
                        {
                            log.LogTransaction.Info("TransactionID : " + TransactionID + " Already Sent To Core Banking System");
                            continue;

                        }

                    }
                    catch (Exception)
                    {
                        log.LogTransaction.Error("Error in fetching TransactionID : " + TransactionID + " from ZagDB");
                    }
                    try
                    {
                        try
                        {


                            log.LogTransaction.Info("TransactionID : " + TransactionID + " Will be sent to Core Banking System");
                            int removedRows = BankIntegrationTransaction1.DeleteQuery(TransactionID);
                            if (removedRows > 0)
                            {
                                log.LogTransaction.Info("TransactionID : " + TransactionID + " removed " + removedRows + " stale detail record(s) prior to sending.");
                            }

                        }
                        catch (Exception)
                        {
                            log.LogTransaction.Error("Error in deleting TransactionID : " + TransactionID + " from ZagDB");
                        }
                        log.LogTransaction.Info("Running Prepare Transaction SP for TransactionID : " + TransactionID);
                        await RunPrepareTransactionSPAsync(TransactionID);

                    }
                    catch (Exception)
                    {
                        PublicInfo.log.LogTransaction.Error("Error in Running Prepare Transaction SP for TransactionID : " + TransactionID);
                    }
                    string result = SendTransactionToCoreBanking(TransactionID);
                    //    If  result.StartsWith ("ERR|")  Then
                    //         SendTransactionConfirmationToZagTrader(TransactionID, Trim(result))
                    //         TransactionDA.InsertQuery(TransactionID, Now, Trim(result))
                    //Else
                    if (result != "NO" && result != "ERR")
                    {
                        log.LogTransaction.Info("TransactionID : " + TransactionID + " Sent Successfully to Core Banking System");
                        SendTransactionConfirmationToZagTrader(TransactionID, result.Trim());
                        log.LogTransaction.Info("TransactionID : " + TransactionID + " Confirmation Sent to Zag Trader");
                        await TransactionsSentBankingSystem.InsertQuery(TransactionID, DateTime.Now, result.Trim()).ConfigureAwait(false);
                        log.LogTransaction.Info("TransactionID : " + TransactionID + " Inserted Successfully to TransactionsSentBankingSystem Table");

                    }
                    //End If

                    //  Exit Sub
                    // End If


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " : Error In Sending Transactions To Core Banking :" + ex.ToString());
                log.LogTransaction.Error("Error In Sending Transactions To Core Banking :" + ex.ToString());
            }

        }
        public static async Task RunPrepareTransactionSPAsync(string transactionId, System.Threading.CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Transaction ID is required.", nameof(transactionId));

            const string StoredProcName = "TransactionsDetailsToSyncWithBankingSystem";

            try
            {
                using (var conn = new SqlConnection(DbConfig.ConnectionString))
                using (var cmd = new SqlCommand(StoredProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (int.TryParse(ConfigurationManager.AppSettings["DbCommandTimeoutSeconds"], out var timeoutSeconds) && timeoutSeconds > 0)
                    {
                        cmd.CommandTimeout = timeoutSeconds;
                    }

                    var p = cmd.Parameters.Add("@TransactionID", SqlDbType.NVarChar, 50);
                    p.Value = transactionId;

                    await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
                    int affected = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                    log.LogTransaction.Info(
                        $"[Async] TransactionID: {transactionId} prepared successfully. SP={StoredProcName}, AffectedRows={affected}");
                }
            }
            catch (SqlException ex)
            {
                log.LogTransaction.Error(
                    $"[Async] SQL error while preparing TransactionID {transactionId} via {StoredProcName}: {ex}");
                throw;
            }
            catch (Exception ex)
            {
                log.LogTransaction.Error(
                    $"[Async] Unexpected error while preparing TransactionID {transactionId} via {StoredProcName}: {ex}");
                throw;
            }
        }
        public static DataSet ParsXMLToDs(string str)
        {
            try
            {
                System.IO.StringReader tmp = new System.IO.StringReader(str);
                DataSet ds = new DataSet();
                ds.ReadXml(tmp);
                return ds;
            }
            catch (Exception)
            {
            }
            return null;


        }
        public static string SendTransactionToCoreBanking(string TransactionID)
        {
            try
            {
                string str = ZAGIBMMQ.Instance.HandelADDUserTransActionsFromDB(TransactionID);
                string id;
                id = TransactionID;
                //    id = "ZT" & "_" & id.Replace("-", "_")
                // TransactionID = id

                if (TransactionsHs.ContainsKey(TransactionID) == false)
                {
                    TransactionsHs.Add(TransactionID, 1);
                }
                if (str.StartsWith("ERR|"))
                {
                    if (TransactionsHs.ContainsKey(TransactionID) == false)
                    {
                        TransactionsHs.Add(TransactionID, 1);
                    }
                    if (TransactionsHs[TransactionID] >= int.Parse(ConfigurationManager.AppSettings["NumberOfRetries"]))
                    {
                        BankIntegrationTransaction1.UpdateRejection(str, TransactionID);
                    }
                    else
                    {
                        TransactionsHs[TransactionID] += 1;

                    }


                    return "NO";
                    // Return  str
                }

                if (str == "Rej")
                {
                    return "ERR";
                }
                DataSet result = ParsXMLToDs(str);
                if (result == null)
                {
                    log.LogTransaction.Error($"Failed to parse core banking response for TransactionID : {TransactionID}. Raw message: {str}");
                    return "ERR";
                }

                if (!result.Tables.Contains("HostTransaction"))
                {
                    log.LogTransaction.Error($"HostTransaction table missing in response for TransactionID : {TransactionID}.");
                    return "ERR";
                }

                DataTable hostTable = result.Tables["HostTransaction"];
                if (hostTable.Rows.Count == 0 || hostTable.Columns.Count <= 1)
                {
                    log.LogTransaction.Error($"HostTransaction table missing expected data for TransactionID : {TransactionID}.");
                    return "ERR";
                }

                string status = hostTable.Rows[0][1].Nvl(string.Empty);
                if (string.Equals(status, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    if (result.Tables.Contains("TrnIdentifier"))
                    {
                        DataTable identifierTable = result.Tables["TrnIdentifier"];
                        if (identifierTable.Rows.Count > 0)
                        {
                            if (identifierTable.Columns.Contains("TrnId"))
                            {
                                return identifierTable.Rows[0]["TrnId"].Nvl(string.Empty);
                            }

                            if (identifierTable.Columns.Count > 0)
                            {
                                return identifierTable.Rows[0][0].Nvl(string.Empty);
                            }
                        }
                    }

                    log.LogTransaction.Error($"TransactionID : {TransactionID} missing TrnIdentifier.TrnId in response.");
                    return "ERR";
                }

                string rejectionMessage = "Core banking rejected the transaction without details.";
                if (result.Tables.Contains("ErrorDetail"))
                {
                    DataTable errorTable = result.Tables["ErrorDetail"];
                    if (errorTable.Rows.Count > 0)
                    {
                        if (errorTable.Columns.Count > 1)
                        {
                            rejectionMessage = errorTable.Rows[0][1].Nvl(rejectionMessage);
                        }
                        else
                        {
                            rejectionMessage = errorTable.Rows[0][0].Nvl(rejectionMessage);
                        }
                    }
                }
                else
                {
                    log.LogTransaction.Warn($"ErrorDetail table missing in response for TransactionID : {TransactionID}.");
                }

                BankIntegrationTransaction1.UpdateRejection(rejectionMessage, TransactionID);
                return "NO";
            }
            catch (Exception ex)
            {
                log.LogTransaction.Error(ex.ToString());
            }
            return "ERR";

        }

        public static void SendTransactionConfirmationToZagTrader(string TransactionID, string TransDesc)
        {
            try
            {
                log.LogTransaction.Info("Sending Transaction Confirmation to Zag Trader for TransactionID : " + TransactionID);
                BankIntegrationTransaction1.UpdateQuery(TransDesc, TransactionID);
            }
            catch (Exception)
            {
            }

        }

    }

}
