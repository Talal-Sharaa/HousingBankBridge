// VBConversions Note: VB project level imports
using IBM.WMQ;
using IBM.XMS;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
// End of VB project level imports

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using ZagAPIServer;
using ZagAPIServer.DB.DSZAG;
using ZagAPIServer.Properties;
using ZagAPIServer.Tools;


namespace ZagAPIServer
{
    sealed class IBMMQ
    {

    }



    public class ZAGIBMMQ
    {
        private static ZAGIBMMQ _Instance = null;
        public static ZAGIBMMQ Instance
        {
            get
            {
                try
                {
                    if (ReferenceEquals(_Instance, null))
                    {
                        _Instance = new ZAGIBMMQ();

                    }
                    return _Instance;
                }
                catch (Exception ex)
                {
                    PublicInfo.log.LogApp.Error(ex.ToString());
                    return null; // Add this line to ensure a return value
                }

            }
            set
            {
                try
                {
                    _Instance = value;
                }
                catch (Exception ex)
                {
                    PublicInfo.log.LogApp.Error(ex.ToString());
                }


            }
        }
        public void GetDataGenral(string serviceRequest)
        {
            try
            {
                XMSFactoryFactory ff = default(XMSFactoryFactory);
                ff = XMSFactoryFactory.GetInstance(XMSC.CT_WPM);
                IConnection connectionWPM = default(IConnection);
                ISession sessionWPM = default(ISession);
                IDestination destination = default(IDestination);
                Requestor Requestor = default(Requestor);
                ITextMessage textRequestMessage = default(ITextMessage);
                ITextMessage textResponseMessage = default(ITextMessage);

                IConnectionFactory cf = default(IConnectionFactory);
                cf = ff.CreateConnectionFactory();
                cf.SetStringProperty(XMSC.WPM_BUS_NAME, System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]));
                cf.SetStringProperty(XMSC.WPM_HOST_NAME, System.Convert.ToString(ConfigurationManager.AppSettings["HostName"]));
                cf.SetStringProperty(XMSC.WPM_PORT, System.Convert.ToString(ConfigurationManager.AppSettings["PORT"]));
                cf.SetStringProperty(XMSC.WPM_TARGET_TRANSPORT_CHAIN, System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]));

                connectionWPM = cf.CreateConnection();
                Console.WriteLine("Connection created");

                sessionWPM = connectionWPM.CreateSession(false, AcknowledgeMode.AutoAcknowledge);
                Console.WriteLine("Session created");

                destination = sessionWPM.CreateQueue(ReqQueus.brkBalInq_Req);
                Console.WriteLine("Created destination : " + destination.Name);

                Requestor = new Requestor(sessionWPM, destination);

                connectionWPM.Start();


                textRequestMessage = sessionWPM.CreateTextMessage(serviceRequest);

                //'  // Send the request And wait for response.
                //  // NOTE: SimpleRequestorServer must be running To receive response, othewise
                //   // the call will hang.
                Console.WriteLine("Submitting request");
                textResponseMessage = (ITextMessage)Requestor.Request(textRequestMessage);

                //  // Process the response Or simply print it.
                Console.Write("Response Received:\\n" + textResponseMessage.Text + "\\n");

                // // Cleanup
                Requestor.Dispose();
                destination.Dispose();
                sessionWPM.Dispose();
                connectionWPM.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }




        }
        public string HandelADDUserTransActionsFromDB(string TransactionID)
        {


            VBMath.Randomize();

            string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
            string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
            string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
            string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
            string queue = ReqQueus.brkXferTranAdd_Req;
            string queuereply = ReplyQueus.brkXferTranAdd_Rply;
            QueueINFO req = new QueueINFO();
            req.channelName = channel;
            req.connectionName = connection;
            req.QueueManager = queuemanager;
            req.QueueName = queue;
            req.QueueNameReply = queuereply;
            bool JustReadTheMsg = false;

            //Req_Zag_Balnce_X
            // Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)

            string requetdata = Resource1.SendTransActions;
            // Dim id As String
            string originalID = TransactionID;

            string id = "";
            id = TransactionID;
            id = "ZT" + "_" + id.Replace("-", "_");
            if (PublicInfo.TransactionsHs.ContainsKey(originalID))
            {
                int Rstr = PublicInfo.TransactionsHs[originalID];
                if (Rstr == 0)
                {
                    //Try  to  Read it  again
                    JustReadTheMsg = true;

                }
                else
                {
                    PublicInfo.log.LogTransaction.Info("RESENDING  Transaction ID : " + id);
                    if (PublicInfo.IncremntalIDs.ContainsKey(originalID) == false)
                    {
                        PublicInfo.IncremntalIDs.Add(originalID, 0);

                    }
                    PublicInfo.IncremntalIDs[originalID]++;

                    id = id + "_" + System.Convert.ToString(PublicInfo.IncremntalIDs[originalID]);



                }
            }


            // Public MesgIDs As  New  Dictionary(Of String ,  Byte()  )
            if (PublicInfo.MesgIDs.ContainsKey(id) == false)
            {
                PublicInfo.MesgIDs.Add(id, null);



            }

            // id = System.Guid.NewGuid.ToString("N")
            // id = id.Substring(0, 27)


            requetdata = requetdata.Replace("Req_Zag_ADD_Trans_X", id);
            DataTable xDTBankIntegrationTransaction1 = BankIntegrationTransaction1.Fill(TransactionID).GetAwaiter().GetResult();

            //Req_Zag_ADD_Trans_X

            if (xDTBankIntegrationTransaction1.Rows.Count == 0)
            {
                return "Rej";
            }


            StringBuilder strLegs = new StringBuilder();
            int k = 1;
            HashSet<string> xHashUniqeLeg = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i <= xDTBankIntegrationTransaction1.Rows.Count - 1; i++)
            {
                string tmp = Resource1.TransLeg;
                if (xDTBankIntegrationTransaction1.Rows[i]["CreditBankAccountID"].Nvl("") != "0")
                {
                    PublicInfo.log.LogTransaction.Info("Processing Credit Leg");
                    //ZAG1CURRENCY
                    DateTime transdate = default(DateTime);
                    try
                    {
                        transdate = System.Convert.ToDateTime(xDTBankIntegrationTransaction1.Rows[i]["ValueDate"]);
                        transdate = transdate.AddSeconds(10);

                        // If transdate < Now Then transdate = Now


                    }
                    catch (Exception)
                    {
                        transdate = DateTime.Now;

                    }

                    tmp = tmp.Replace("ZAGACCID", xDTBankIntegrationTransaction1.Rows[i]["CreditBankAccountID"].Nvl(""));
                    tmp = tmp.Replace("ZAGFLAG", "C");
                    tmp = tmp.Replace("ZAGAMOUNT", xDTBankIntegrationTransaction1.Rows[i]["CreditAmount"].Nvl(""));
                    tmp = tmp.Replace("ZAGCURRENCY", (xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol2"]).Nvl("").Trim());
                    tmp = tmp.Replace("ZAG1CURRENCY", (xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol"]).Nvl("").Trim()); // "USD")
                    tmp = tmp.Replace("ZAGDATE", transdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
                    try
                    {
                        string tmpstr = System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["GLDesc"]);
                        if (tmpstr.Length > 20)
                        {
                            tmpstr = System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["GLDesc"].Nvl("").Substring(0, 20));
                        }
                        tmp = tmp.Replace("ZAGTRNPRT", tmpstr);
                    }
                    catch (Exception)
                    {
                        tmp = tmp.Replace("ZAGTRNPRT", "No Desc");

                    }

                    if (Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol"])) == Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol2"])))
                    {
                        tmp = tmp.Replace("<Rate>ZAGRATE</Rate>", "<Rate />"); // Trim(tbl.Rows(i)("CurrencyExchangeRateUSD")))

                        tmp = tmp.Replace("<RateCode>ZAG1RATECODE</RateCode>", "<RateCode />"); // tbl.Rows(i)("RateCode"))
                    }
                    else
                    {
                        tmp = tmp.Replace("ZAGRATE", Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencyExchangeRateUSD"])));

                        tmp = tmp.Replace("ZAG1RATECODE", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["RateCode"]));
                    }
                    string number = Strings.Format(k, "00000");


                    if (xHashUniqeLeg.Add(tmp))
                    {
                        tmp = tmp.Replace("ZAGSERIAL", number);
                    }
                    else
                    {
                        tmp = "";
                    }
                }
                else
                {
                    DateTime transdate = default(DateTime);
                    try
                    {
                        transdate = System.Convert.ToDateTime(xDTBankIntegrationTransaction1.Rows[i]["ValueDate"]);
                        transdate = transdate.AddSeconds(10);

                        //  If transdate < Now Then transdate = Now


                    }
                    catch (Exception)
                    {
                        transdate = DateTime.Now;

                    }


                    // Dim tmp As String = My.Resources.TransLeg
                    tmp = tmp.Replace("ZAGACCID", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["DebitBankAccountID"]));
                    tmp = tmp.Replace("ZAGFLAG", "D");
                    tmp = tmp.Replace("ZAGAMOUNT", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["DebitAmount"]));
                    tmp = tmp.Replace("ZAGCURRENCY", Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol2"])));
                    tmp = tmp.Replace("ZAG1CURRENCY", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol"])); // "USD")
                    try
                    {
                        string tmpstr = System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["GLDesc"]);
                        if (tmpstr.Length > 20)
                        {
                            tmpstr = System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["GLDesc"].Nvl("").Substring(0, 20));
                        }
                        tmp = tmp.Replace("ZAGTRNPRT", tmpstr);
                        // tmp = tmp.Replace("ZAGTRNPRT", (tbl.Rows(i)("GLDesc").ToString().Substring(0, 20)))
                    }
                    catch (Exception)
                    {
                        tmp = tmp.Replace("ZAGTRNPRT", "NO Desc ");
                    }

                    tmp = tmp.Replace("ZAGDATE", transdate.ToString("yyyy-MM-ddThh:mm:ss.fff"));

                    if (Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol"])) == Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencySymbol2"])))
                    {
                        tmp = tmp.Replace("<Rate>ZAGRATE</Rate>", "<Rate />"); // Trim(tbl.Rows(i)("CurrencyExchangeRateUSD")))

                        tmp = tmp.Replace("<RateCode>ZAG1RATECODE</RateCode>", "<RateCode />"); // tbl.Rows(i)("RateCode"))
                    }
                    else
                    {
                        tmp = tmp.Replace("ZAGRATE", Strings.Trim(System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CurrencyExchangeRateUSD"])));

                        tmp = tmp.Replace("ZAG1RATECODE", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["RateCode"]));
                    }


                    string number = Strings.Format(k, "00000");


                    if (xHashUniqeLeg.Add(tmp))
                    {
                        tmp = tmp.Replace("ZAGSERIAL", number);
                    }
                    else
                    {
                        tmp = "";
                    }

                }

                //k = k + 1
                //strLegs.Append(tmp & vbNewLine)
                //tmp = My.Resources.TransLeg
                //tmp = tmp.Replace("ZAGACCID", tbl.Rows(k)("DebitBankAccountID"))
                //tmp = tmp.Replace("ZAGFLAG", "D")
                //tmp = tmp.Replace("ZAGAMOUNT", tbl.Rows(k)("DebitAmount"))
                //tmp = tmp.Replace("ZAGCURRENCY", tbl.Rows(k)("CurrencySymbol"))
                //tmp = tmp.Replace("ZAGDATE", Now.ToString("yyyy-MM-ddThh:mm:ss.fff"))
                //number = Format(k, "00000")


                //tmp = tmp.Replace("ZAGSERIAL", number)
                if (tmp != "")
                {
                    k++;
                    strLegs.Append(tmp + Constants.vbNewLine);
                }


            }
            requetdata = requetdata.Replace("ZAGLEGS", strLegs.ToString());

            k = 1;
            StringBuilder strTails = new StringBuilder();
            int num = System.Convert.ToInt32(10000000 + VBMath.Rnd() * 999999);

            for (int i = 0; i <= xDTBankIntegrationTransaction1.Rows.Count - 1; i++)
            {
                string tmp = Resource1.TransTail;
                if (xDTBankIntegrationTransaction1.Rows[i]["CreditBankAccountID"].Nvl("") != "0")
                {
                    tmp = tmp.Replace("ZAGPURPOSCODE", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["CRReasonCode"]));
                    tmp = tmp.Replace("ZAGTIME", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                    tmp = tmp.Replace("ZAGID", System.Convert.ToString(num));
                    try
                    {
                        if (Information.IsDBNull(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]))
                        {
                            tmp = tmp.Replace("ZAGEMP", "BRK");
                        }
                        else
                        {
                            tmp = tmp.Replace("ZAGEMP", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]));

                        }


                    }
                    catch (Exception)
                    {
                        tmp = tmp.Replace("ZAGEMP", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]));
                    }
                    string number = Strings.Format(k, "00000");
                    if (xHashUniqeLeg.Add(tmp))
                    {
                        tmp = tmp.Replace("ZAGSERIAL", number);
                    }
                    else
                    {
                        tmp = "";
                    }
                }
                else
                {
                    tmp = tmp.Replace("ZAGPURPOSCODE", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["DBReasonCode"]));
                    tmp = tmp.Replace("ZAGTIME", DateTime.Now.ToString("yyyyMMddhhmmssfff"));
                    tmp = tmp.Replace("ZAGID", System.Convert.ToString(num));
                    try
                    {
                        if (Information.IsDBNull(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]))
                        {
                            tmp = tmp.Replace("ZAGEMP", "BRK");
                        }
                        else
                        {
                            tmp = tmp.Replace("ZAGEMP", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]));

                        }


                    }
                    catch (Exception)
                    {
                        tmp = tmp.Replace("ZAGEMP", System.Convert.ToString(xDTBankIntegrationTransaction1.Rows[i]["ZAGEMP"]));
                    }
                    string number = Strings.Format(k, "00000");
                    if (xHashUniqeLeg.Add(tmp))
                    {
                        tmp = tmp.Replace("ZAGSERIAL", number);
                    }
                    else
                    {
                        tmp = "";
                    }


                }


                if (tmp != "")
                {
                    k++;
                    strTails.Append(tmp + Constants.vbNewLine); 
                }
                //tmp = My.Resources.TransTail
                //tmp = tmp.Replace("ZAGPURPOSCODE", tbl.Rows(i)("DBReasonCode"))
                //tmp = tmp.Replace("ZAGTIME", Now.ToString("yyyyMMddhhmmssfff"))
                //tmp = tmp.Replace("ZAGID", num + 1)
                //number = Format(k, "00000")

                //tmp = tmp.Replace("ZAGSERIAL", number)
                //k = k + 1
                //strTails.Append(tmp & vbNewLine)


            }

            requetdata = requetdata.Replace("ZAGTAIL", strTails.ToString());
            requetdata = requetdata.Replace("ZAGDATE", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff"));
            requetdata = requetdata.Replace("SMS", channelID);

            //requetdata = File.ReadAllText("e:\tran_req.xml")
            //requetdata = requetdata.Replace("Zag", id)
            Console.WriteLine(DateTime.Now + " : Sending Transaction To Core With Transaction ID :" + TransactionID);
            PublicInfo.log.LogTransaction.Info("Sending Transaction :" + requetdata);
            string str = ZAGIBMMQ.Instance.GetDataFromIBMMQDirect(req, requetdata, JustReadTheMsg, originalID);
            PublicInfo.log.LogTransactionR.Info("Transaction Response : " + str);

            return str;




        }


        public string GetDataFromIBMMQForBallance(QueueINFO RequestInfo, string RequestData)
        {

            return GetDataFromIBMMQDirect(RequestInfo, RequestData);

        }

        public System.Threading.Timer MyTimer;


        public string GetDataFromIBMMQForGetTransactions(QueueINFO RequestInfo, string RequestData)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UsePreOpenQ"]))
            {
            }
            else
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);

            }
            int addressofAvi = GetOpengetTransactionsQueu();
            if (addressofAvi == -1)
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);
            }

            // Name of queue to useh
            HsGetTransactionsOfConnections[addressofAvi].IsAvialable = false;

            if (string.IsNullOrEmpty(RequestInfo.QueueManager))
            {
                MessageBox.Show("Usage: SendToWebSphere <queue> <xml_Filename>");
                return " -1";
            }
            else
            {
                //  HsBallanceOfConnections(addressofAvi).queueName = RequestInfo.QueueName
            }
            // Dim queuIn As MQQueue


            try
            {

                //Dim QueueManager As [String] = RequestInfo.QueueManager
                //Dim channelName As [String] = RequestInfo.channelName
                //Dim connectionName As [String] = RequestInfo.connectionName
                //mqQMgr = New MQQueueManager(QueueManager, channelName, connectionName)
            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }

            //
            // Try to open the queue
            //
            try
            {
                // open queue for output
                // but not if MQM stopping
                //mqQueue = mqQMgr.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING)
                //queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "")

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }
            MQMessage mqMsg = default(MQMessage);
            MQMessage mqMsgOut = default(MQMessage);
            MQPutMessageOptions mqPutMsgOpts;
            MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
            mqMsg = new MQMessage();
            mqMsgOut = new MQMessage();
            mqMsg.CharacterSet = 1208;
            // Set UTF-8 encoding

            mqMsg.Persistence = 1;

            mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
            mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
            mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;
            mqPutMsgOpts = new MQPutMessageOptions();


            mqMsg.ReplyToQueueName = System.Convert.ToString(HsGetTransactionsOfConnections[addressofAvi].queuIn.Name);

            mqMsg.WriteString(RequestData);


            try
            {
                //  HsGetTransactionsOfConnections(addressofAvi).
                HsGetTransactionsOfConnections[addressofAvi].mqQueue.Put(mqMsg);
                HsGetTransactionsOfConnections[addressofAvi].mqQueue.Close();
                try
                {
                    mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                    mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                    mqGetMsgOpts.WaitInterval = 10000;
                    mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;


                    try
                    {
                        HsGetTransactionsOfConnections[addressofAvi].queuIn.Get(mqMsgOut, mqGetMsgOpts);
                        // mqMsgOut.CorrelationId
                        string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                        try
                        {

                            // mqQueue.Close()
                            // HsGetTransactionsOfConnections(addressofAvi).IsAvialable = True

                            HsGetTransactionsOfConnections[addressofAvi].mqQMgr.Disconnect();
                            HsGetTransactionsOfConnections[addressofAvi].queuIn.Close();
                            BalanceConnectionsClosedConnections.Enqueue(HsGetTransactionsOfConnections[addressofAvi]);


                            // mqQMgr.Disconnect()
                        }
                        catch (MQException)
                        {
                            return "ERR";

                        }


                        return str;

                    }
                    catch (MQException mqe)
                    {
                        return (mqe.Message);

                    }

                }
                catch (Exception)
                {

                }
            }
            catch (MQException)
            {
                // report the error
                return "Err";

            }
            return "Success";

        }
        public string GetDataFromIBMMQForAddTransactions(QueueINFO RequestInfo, string RequestData)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UsePreOpenQ"]))
            {
            }
            else
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);

            }
            int addressofAvi = GetOpenAddTransQueu();
            if (addressofAvi == -1)
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);
            }

            // Name of queue to useh
            HsAddTransactionsOfConnections[addressofAvi].IsAvialable = false;

            if (string.IsNullOrEmpty(RequestInfo.QueueManager))
            {
                // MsgBox("Usage: SendToWebSphere <queue> <xml_Filename>")
                return " -1";
            }
            else
            {
                //  HsBallanceOfConnections(addressofAvi).queueName = RequestInfo.QueueName
            }
            // Dim queuIn As MQQueue


            try
            {

                //Dim QueueManager As [String] = RequestInfo.QueueManager
                //Dim channelName As [String] = RequestInfo.channelName
                //Dim connectionName As [String] = RequestInfo.connectionName
                //mqQMgr = New MQQueueManager(QueueManager, channelName, connectionName)
            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }

            //
            // Try to open the queue
            //
            try
            {
                // open queue for output
                // but not if MQM stopping
                //mqQueue = mqQMgr.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING)
                //queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "")

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }
            MQMessage mqMsg = default(MQMessage);
            MQMessage mqMsgOut = default(MQMessage);
            MQPutMessageOptions mqPutMsgOpts;
            MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
            mqMsg = new MQMessage();
            mqMsgOut = new MQMessage();
            mqMsg.CharacterSet = 1208;
            // Set UTF-8 encoding

            mqMsg.Persistence = 1;

            mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
            mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
            mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;
            mqPutMsgOpts = new MQPutMessageOptions();


            mqMsg.ReplyToQueueName = System.Convert.ToString(HsAddTransactionsOfConnections[addressofAvi].queuIn.Name);

            mqMsg.WriteString(RequestData);


            try
            {
                //  HsAddTransactionsOfConnections(addressofAvi).
                HsAddTransactionsOfConnections[addressofAvi].mqQueue.Put(mqMsg);
                HsAddTransactionsOfConnections[addressofAvi].mqQueue.Close();
                try
                {
                    mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                    mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                    mqGetMsgOpts.WaitInterval = 10000;
                    mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;


                    try
                    {
                        HsAddTransactionsOfConnections[addressofAvi].queuIn.Get(mqMsgOut, mqGetMsgOpts);
                        // mqMsgOut.CorrelationId
                        string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                        try
                        {

                            // mqQueue.Close()
                            // HsAddTransactionsOfConnections(addressofAvi).IsAvialable = True

                            HsAddTransactionsOfConnections[addressofAvi].mqQMgr.Disconnect();
                            HsAddTransactionsOfConnections[addressofAvi].queuIn.Close();
                            BalanceConnectionsClosedConnections.Enqueue(HsAddTransactionsOfConnections[addressofAvi]);


                            // mqQMgr.Disconnect()
                        }
                        catch (MQException)
                        {
                            return "ERR";

                        }


                        return str;

                    }
                    catch (MQException mqe)
                    {
                        return (mqe.Message);

                    }

                }
                catch (Exception)
                {

                }
            }
            catch (MQException)
            {
                // report the error
                return "Err";

            }
            return "Success";
        }

        public string GetDataFromIBMMQForAddLien(QueueINFO RequestInfo, string RequestData)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UsePreOpenQ"]))
            {
            }
            else
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);

            }
            int addressofAvi = GetOpenAddTransQueu();
            if (addressofAvi == -1)
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);
            }

            // Name of queue to useh
            HsAddLianOfConnections[addressofAvi].IsAvialable = false;

            if (string.IsNullOrEmpty(RequestInfo.QueueManager))
            {
                // MsgBox("Usage: SendToWebSphere <queue> <xml_Filename>")
                return " -1";
            }
            else
            {
                //  HsBallanceOfConnections(addressofAvi).queueName = RequestInfo.QueueName
            }
            // Dim queuIn As MQQueue


            try
            {

                //Dim QueueManager As [String] = RequestInfo.QueueManager
                //Dim channelName As [String] = RequestInfo.channelName
                //Dim connectionName As [String] = RequestInfo.connectionName
                //mqQMgr = New MQQueueManager(QueueManager, channelName, connectionName)
            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }

            //
            // Try to open the queue
            //
            try
            {
                // open queue for output
                // but not if MQM stopping
                //mqQueue = mqQMgr.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING)
                //queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "")

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }
            MQMessage mqMsg = default(MQMessage);
            MQMessage mqMsgOut = default(MQMessage);
            MQPutMessageOptions mqPutMsgOpts;
            MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
            mqMsg = new MQMessage();
            mqMsgOut = new MQMessage();
            mqMsg.CharacterSet = 1208;
            // Set UTF-8 encoding

            mqMsg.Persistence = 1;

            mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
            mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
            mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;
            mqPutMsgOpts = new MQPutMessageOptions();


            mqMsg.ReplyToQueueName = System.Convert.ToString(HsAddLianOfConnections[addressofAvi].queuIn.Name);

            mqMsg.WriteString(RequestData);


            try
            {
                //  HsAddLianOfConnections(addressofAvi).
                HsAddLianOfConnections[addressofAvi].mqQueue.Put(mqMsg);
                HsAddLianOfConnections[addressofAvi].mqQueue.Close();
                try
                {
                    mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                    mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                    mqGetMsgOpts.WaitInterval = 10000;
                    mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;


                    try
                    {
                        HsAddLianOfConnections[addressofAvi].queuIn.Get(mqMsgOut, mqGetMsgOpts);
                        // mqMsgOut.CorrelationId
                        string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                        try
                        {

                            // mqQueue.Close()
                            // HsAddLianOfConnections(addressofAvi).IsAvialable = True

                            HsAddLianOfConnections[addressofAvi].mqQMgr.Disconnect();
                            HsAddLianOfConnections[addressofAvi].queuIn.Close();
                            BalanceConnectionsClosedConnections.Enqueue(HsAddLianOfConnections[addressofAvi]);


                            // mqQMgr.Disconnect()
                        }
                        catch (MQException)
                        {
                            return "ERR";

                        }


                        return str;

                    }
                    catch (MQException mqe)
                    {
                        return (mqe.Message);

                    }

                }
                catch (Exception)
                {

                }
            }
            catch (MQException)
            {
                // report the error
                return "Err";

            }
            return "Success";
        }

        public string GetDataFromIBMMQForModifyLien(QueueINFO RequestInfo, string RequestData)
        {
            if (bool.Parse(ConfigurationManager.AppSettings["UsePreOpenQ"]))
            {
            }
            else
            {
                PublicInfo.log.LogRemoveLEAN.Info(RequestData);

                return GetDataFromIBMMQDirect(RequestInfo, RequestData);

            }
            int addressofAvi = GetOpenAddTransQueu();
            if (addressofAvi == -1)
            {
                return GetDataFromIBMMQDirect(RequestInfo, RequestData);
            }

            // Name of queue to useh
            HsModifyOfConnections[addressofAvi].IsAvialable = false;

            if (string.IsNullOrEmpty(RequestInfo.QueueManager))
            {
                // MsgBox("Usage: SendToWebSphere <queue> <xml_Filename>")
                return " -1";
            }
            else
            {
                //  HsBallanceOfConnections(addressofAvi).queueName = RequestInfo.QueueName
            }
            // Dim queuIn As MQQueue


            try
            {

                //Dim QueueManager As [String] = RequestInfo.QueueManager
                //Dim channelName As [String] = RequestInfo.channelName
                //Dim connectionName As [String] = RequestInfo.connectionName
                //mqQMgr = New MQQueueManager(QueueManager, channelName, connectionName)
            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }

            //
            // Try to open the queue
            //
            try
            {
                // open queue for output
                // but not if MQM stopping
                //mqQueue = mqQMgr.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING)
                //queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "")

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                return System.Convert.ToString(mqe.Reason);
            }
            MQMessage mqMsg = default(MQMessage);
            MQMessage mqMsgOut = default(MQMessage);
            MQPutMessageOptions mqPutMsgOpts;
            MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
            mqMsg = new MQMessage();
            mqMsgOut = new MQMessage();
            mqMsg.CharacterSet = 1208;
            // Set UTF-8 encoding

            mqMsg.Persistence = 1;

            mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
            mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
            mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;
            mqPutMsgOpts = new MQPutMessageOptions();


            mqMsg.ReplyToQueueName = System.Convert.ToString(HsModifyOfConnections[addressofAvi].queuIn.Name);

            mqMsg.WriteString(RequestData);


            try
            {
                //  HsModifyOfConnections(addressofAvi).
                HsModifyOfConnections[addressofAvi].mqQueue.Put(mqMsg);
                HsModifyOfConnections[addressofAvi].mqQueue.Close();
                try
                {
                    mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                    mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                    mqGetMsgOpts.WaitInterval = 10000;
                    mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;


                    try
                    {
                        HsModifyOfConnections[addressofAvi].queuIn.Get(mqMsgOut, mqGetMsgOpts);
                        // mqMsgOut.CorrelationId
                        string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                        try
                        {

                            // mqQueue.Close()
                            // HsModifyOfConnections(addressofAvi).IsAvialable = True

                            HsModifyOfConnections[addressofAvi].mqQMgr.Disconnect();
                            HsModifyOfConnections[addressofAvi].queuIn.Close();
                            BalanceConnectionsClosedConnections.Enqueue(HsModifyOfConnections[addressofAvi]);


                            // mqQMgr.Disconnect()
                        }
                        catch (MQException)
                        {
                            return "ERR";

                        }


                        return str;

                    }
                    catch (MQException mqe)
                    {
                        return (mqe.Message);

                    }

                }
                catch (Exception)
                {

                }
            }
            catch (MQException)
            {
                // report the error
                return "Err";

            }
            return "Success";
        }

        public string GetDataFromIBMMQDirect(QueueINFO RequestInfo, string RequestData, bool JustReadTheMsgID = false, string MessageID = "")
        {
            try
            {
                MQQueueManager mqQMgr = default(MQQueueManager);
                // MQQueueManager instance
                MQQueue mqQueue = default(MQQueue);
                // MQQueue instance
                MQMessage mqMsg = default(MQMessage);
                MQMessage mqMsgOut = default(MQMessage);

                //RequestData = RequestData.Replace(vbNewLine, "")
                //RequestData = RequestData.Replace(vbLf, "")
                //RequestData = RequestData.Replace(vbCr, "")
                // MQMessage instance
                MQPutMessageOptions mqPutMsgOpts;
                MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
                // MQPutMessageOptions instance
                string queueName = "";
                // Name of queue to use
                if (string.IsNullOrEmpty(RequestInfo.QueueManager))
                {
                    PublicInfo.log.LogApp.Error("Queue Manage Empty ");
                }
                else
                {
                    queueName = RequestInfo.QueueName;
                }
                MQQueue queuIn = default(MQQueue);
                string QueueManager = RequestInfo.QueueManager;
                string channelName = RequestInfo.channelName;
                string connectionName = RequestInfo.connectionName;

                try
                {


                    mqQMgr = new MQQueueManager(QueueManager, channelName, connectionName);
                }
                catch (MQException mqe)
                {
                    // stop if failed
                    Console.WriteLine("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);
                    Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                    PublicInfo.log.LogApp.Error("create of MQQueueManager ended with " + mqe.ToString());
                    PublicInfo.log.LogVoice.Error("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);
                    return "-1|Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName + "|" + mqe.Message;

                }

                //
                // Try to open the queue
                //
                try
                {
                    // open queue for output
                    // but not if MQM stopping
                    mqQueue = mqQMgr.AccessQueue(queueName, IBM.WMQ.MQC.MQOO_OUTPUT + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING);
                    queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, IBM.WMQ.MQC.MQOO_INPUT_AS_Q_DEF + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "");

                }
                catch (MQException mqe)
                {
                    // stop if failed
                    Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                    PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                    PublicInfo.log.LogVoice.Error("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);


                    return System.Convert.ToString(mqe.Reason);
                }

                mqMsg = new MQMessage();

                mqMsg.CharacterSet = 1208;
                // Set UTF-8 encoding

                mqMsg.Persistence = 1;

                mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
                mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
                mqPutMsgOpts = new MQPutMessageOptions();

                mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;

                mqMsg.ReplyToQueueName = queuIn.Name;

                mqMsg.WriteString(RequestData);

                try
                {
                    if (JustReadTheMsgID)
                    {
                        if (PublicInfo.MesgIDs.ContainsKey(MessageID))
                        {
                            mqMsg.MessageId = PublicInfo.MesgIDs[MessageID];
                        }
                        else
                        {
                            mqQueue.Put(mqMsg);

                        }

                    }
                    else
                    {
                        if (MessageID != "")
                        {
                            if (PublicInfo.MesgIDs.ContainsKey(MessageID) == false)
                            {
                                mqQueue.Put(mqMsg);
                            }
                            else
                            {
                                if (ReferenceEquals(PublicInfo.MesgIDs[MessageID], null) == true)
                                {
                                    mqQueue.Put(mqMsg);
                                }
                                else
                                {
                                    mqMsg.MessageId = PublicInfo.MesgIDs[MessageID];

                                }
                            }

                        }
                        else
                        {
                            mqQueue.Put(mqMsg);

                        }

                    }
                    //If MessageID <> "" Then
                    //     If  MesgIDs.ContainsKey (  MessageID)  = True
                    //        If  IsNothing ( MesgIDs(MessageID ))  = True    Then
                    //             mqQueue.Put(mqMsg)
                    //            Else
                    //            mqMsg.MessageId  = MesgIDs(MessageID )
                    //        End If
                    //        Else
                    //         mqQueue.Put(mqMsg)

                    //     End If
                    //    Else
                    //    mqQueue.Put(mqMsg)

                    //End If

                    try
                    {
                        PublicInfo.log.LogApp.Info("Message To MQ :" + RequestData);
                        PublicInfo.log.LogApp.Info("Message Sent with  :" + System.Text.ASCIIEncoding.Unicode.GetString(mqMsg.MessageId));

                    }
                    catch (Exception)
                    {
                    }
                    if (MessageID != "")
                    {
                        if (PublicInfo.MesgIDs.ContainsKey(MessageID) == true)
                        {
                            PublicInfo.MesgIDs[MessageID] = mqMsg.MessageId;


                        }
                    }




                    try
                    {
                        mqMsgOut = new MQMessage();
                        mqMsgOut.CorrelationId = mqMsg.MessageId;
                        mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                        mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                        mqGetMsgOpts.WaitInterval = 30000;
                        mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;



                        try
                        {
                            //                        If  raiseerror Then
                            //                        Dim tp As  New  MQException (2 ,2033)
                            //                        Throw tp
                            //End If

                            queuIn.Get(mqMsgOut, mqGetMsgOpts);
                            string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                            try
                            {




                                mqQMgr.Disconnect();
                                mqQMgr.Close();

                                mqQMgr.Disconnect();
                                mqQueue.Close();
                                queuIn.Close();
                                // mqQMgr.Commit()
                                // mqQMgr = Nothing

                            }
                            catch (Exception mqe)
                            {

                                Console.WriteLine("Error In Closing MQ : " + mqe.ToString());

                                mqQMgr.Disconnect();
                                queuIn.Close();
                                PublicInfo.log.LogApp.Error("MQQueueManager::Error ended with " + mqe.ToString());
                                // Return "ERR"

                            }


                            return str;

                        }
                        catch (MQException mqe)
                        {



                            mqQMgr.Disconnect();
                            mqQMgr.Close();

                            queuIn.Close();
                            PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue Error with " + mqe.ToString());
                            PublicInfo.log.LogVoice.Error("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);

                            return ("ERR|" + mqe.Message);

                        }

                    }
                    catch (Exception ex)
                    {



                        mqQMgr.Disconnect();
                        queuIn.Close();
                        PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue Error with " + ex.ToString());
                        PublicInfo.log.LogVoice.Error("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);
                        return ("ERR|" + ex.Message);
                    }
                }
                catch (MQException mqe)
                {
                    PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue Error with " + mqe.ToString());
                    PublicInfo.log.LogVoice.Error("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);

                    // report the error
                    return "Err";

                }
            }
            catch (Exception ex)
            {
                return "-1|Issue In :" + ex.Message;

            }


        }
        public string GetDataFromIBMMQDirectForTestConnection(QueueINFO RequestInfo, string RequestData)
        {
            try
            {
                MQQueueManager mqQMgr = default(MQQueueManager);
                // MQQueueManager instance
                MQQueue mqQueue = default(MQQueue);
                // MQQueue instance
                MQMessage mqMsg = default(MQMessage);
                MQMessage mqMsgOut = default(MQMessage);
                //RequestData = RequestData.Replace(vbNewLine, "")
                //RequestData = RequestData.Replace(vbLf, "")
                //RequestData = RequestData.Replace(vbCr, "")
                // MQMessage instance
                MQPutMessageOptions mqPutMsgOpts;
                MQGetMessageOptions mqGetMsgOpts = default(MQGetMessageOptions);
                // MQPutMessageOptions instance
                string queueName = "";
                // Name of queue to use
                if (string.IsNullOrEmpty(RequestInfo.QueueManager))
                {
                    PublicInfo.log.LogApp.Error("Queue Manage Empty ");
                }
                else
                {
                    queueName = RequestInfo.QueueName;
                }
                MQQueue queuIn = default(MQQueue);
                string QueueManager = RequestInfo.QueueManager;
                string channelName = RequestInfo.channelName;
                string connectionName = RequestInfo.connectionName;

                try
                {


                    mqQMgr = new MQQueueManager(QueueManager, channelName, connectionName);
                }
                catch (MQException mqe)
                {
                    // stop if failed
                    Console.WriteLine("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);
                    Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                    PublicInfo.log.LogApp.Error("create of MQQueueManager ended with " + mqe.ToString());
                    return "-1|Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName + "|" + mqe.Message;

                }

                //
                // Try to open the queue
                //
                try
                {
                    // open queue for output
                    // but not if MQM stopping
                    mqQueue = mqQMgr.AccessQueue(queueName, IBM.WMQ.MQC.MQOO_OUTPUT + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING);
                    queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, IBM.WMQ.MQC.MQOO_INPUT_AS_Q_DEF + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "");

                }
                catch (MQException mqe)
                {
                    // stop if failed
                    Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                    PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                    return System.Convert.ToString(mqe.Reason);
                }

                mqMsg = new MQMessage();

                mqMsg.CharacterSet = 1208;
                // Set UTF-8 encoding

                mqMsg.Persistence = 1;

                mqMsg.Format = IBM.WMQ.MQC.MQFMT_STRING;
                mqMsg.MessageType = IBM.WMQ.MQC.MQMT_REQUEST;
                mqPutMsgOpts = new MQPutMessageOptions();

                mqMsg.Report = IBM.WMQ.MQC.MQRO_COPY_MSG_ID_TO_CORREL_ID;

                mqMsg.ReplyToQueueName = queuIn.Name;

                mqMsg.WriteString(RequestData);

                try
                {
                    mqQueue.Put(mqMsg);
                    mqQueue.Close();

                    try
                    {
                        mqMsgOut = new MQMessage();
                        mqMsgOut.CorrelationId = mqMsg.MessageId;
                        mqGetMsgOpts = new MQGetMessageOptions(); //MessageOptions()
                        mqGetMsgOpts.Options = IBM.WMQ.MQC.MQGMO_WAIT + IBM.WMQ.MQC.MQGMO_FAIL_IF_QUIESCING;
                        mqGetMsgOpts.WaitInterval = 30000;
                        mqGetMsgOpts.MatchOptions = IBM.WMQ.MQC.MQMO_MATCH_CORREL_ID;



                        try
                        {
                            queuIn.Get(mqMsgOut, mqGetMsgOpts);
                            string str = mqMsgOut.ReadString(mqMsgOut.MessageLength);
                            try
                            {

                                queuIn.Close();


                                mqQMgr.Disconnect();
                                try
                                {
                                    mqQMgr.Close();

                                }
                                catch (Exception)
                                {

                                }
                            }
                            catch (MQException mqe)
                            {



                                mqQMgr.Disconnect();
                                queuIn.Close();
                                PublicInfo.log.LogApp.Error("MQQueueManager::Error ended with " + mqe.ToString());
                                return "ERR";

                            }


                            return "1|OK|" + System.Convert.ToString(DateTime.Now);

                        }
                        catch (MQException mqe)
                        {



                            mqQMgr.Disconnect();
                            queuIn.Close();
                            PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue Error with " + mqe.ToString());
                            return (mqe.Message);

                        }

                    }
                    catch (Exception)
                    {



                        mqQMgr.Disconnect();
                        queuIn.Close();
                    }
                }
                catch (MQException mqe)
                {
                    PublicInfo.log.LogApp.Error("MQQueueManager::AccessQueue Error with " + mqe.ToString());
                    // report the error
                    return "Err";

                }
            }
            catch (Exception ex)
            {
                return "-1|Issue In :" + ex.Message;

            }
            return "Success";

        }


        public int GetOpengetTransactionsQueu()
        {
            NumberofConnections = System.Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreOpenQ"]);
            try
            {
                for (int i = 0; i <= NumberofConnections; i++)
                {
                    ZagQueue tmp = HsGetTransactionsOfConnections[i];

                    if (tmp.IsAvialable)
                    {
                        return i;

                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;


        }

        public int GetOpenBallanceQueu()
        {
            NumberofConnections = System.Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreOpenQ"]);
            try
            {
                for (int i = 0; i <= NumberofConnections; i++)
                {
                    ZagQueue tmp = HsBallanceOfConnections[i];

                    if (tmp.IsAvialable)
                    {
                        return i;

                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;


        }

        public int GetOpenAddTransQueu()
        {
            NumberofConnections = System.Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreOpenQ"]);
            try
            {
                for (int i = 0; i <= NumberofConnections; i++)
                {
                    ZagQueue tmp = HsAddTransactionsOfConnections[i];

                    if (tmp.IsAvialable)
                    {
                        return i;

                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;


        }
        public int GetOpenAddLienQueu()
        {
            NumberofConnections = System.Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreOpenQ"]);
            try
            {
                for (int i = 0; i <= NumberofConnections; i++)
                {
                    ZagQueue tmp = HsAddLianOfConnections[i];

                    if (tmp.IsAvialable)
                    {
                        return i;

                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;


        }
        public int GetOpenModifyLienQueu()
        {
            NumberofConnections = System.Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfPreOpenQ"]);
            try
            {
                for (int i = 0; i <= NumberofConnections; i++)
                {
                    ZagQueue tmp = HsModifyOfConnections[i];

                    if (tmp.IsAvialable)
                    {
                        return i;

                    }
                }
            }
            catch (Exception)
            {
            }

            return -1;


        }


        public Dictionary<int, ZagQueue> HsBallanceOfConnections = new Dictionary<int, ZagQueue>();
        public Queue<ZagQueue> BalanceConnectionsClosedConnections = new Queue<ZagQueue>();

        public Dictionary<int, ZagQueue> HsGetTransactionsOfConnections = new Dictionary<int, ZagQueue>();
        public Dictionary<int, ZagQueue> HsAddTransactionsOfConnections = new Dictionary<int, ZagQueue>();

        public Dictionary<int, ZagQueue> HsAddLianOfConnections = new Dictionary<int, ZagQueue>();
        public Dictionary<int, ZagQueue> HsModifyOfConnections = new Dictionary<int, ZagQueue>();


        public int NumberofConnections;

        public ZAGIBMMQ()
        {
            //NumberofConnections = My.Settings.NumberOfPreOpenQ

            //For i As Int32 = 0 To NumberofConnections
            //    Dim queuemanager As String = My.Settings.QueueManager
            //    Dim channel As String = My.Settings.channelName
            //    Dim connection As String = My.Settings.connectionName
            //    Dim channelID As String = My.Settings.ChannelID
            //    Dim queue As String = ReqQueus.brkBalInq_Req
            //    Dim queuereply As String = ReplyQueus.brkBalInq_Rply
            //    Dim req As New QueueINFO
            //    req.channelName = channel
            //    req.connectionName = connection
            //    req.QueueManager = queuemanager
            //    req.QueueName = queue
            //    req.QueueNameReply = queuereply

            //    Dim tmp As New ZagQueue(req)

            //    HsBallanceOfConnections.Add(i, tmp)


            //Next

            //For i As Int32 = 0 To NumberofConnections
            //    Dim queuemanager As String = My.Settings.QueueManager
            //    Dim channel As String = My.Settings.channelName
            //    Dim connection As String = My.Settings.connectionName
            //    Dim channelID As String = My.Settings.ChannelID
            //    Dim queue As String = ReqQueus.brkGetLastNTransactions_Req
            //    Dim queuereply As String = ReplyQueus.brkGetLastNTransactions_Rply
            //    Dim req As New QueueINFO
            //    req.channelName = channel
            //    req.connectionName = connection
            //    req.QueueManager = queuemanager
            //    req.QueueName = queue
            //    req.QueueNameReply = queuereply

            //    Dim tmp As New ZagQueue(req)

            //    HsGetTransactionsOfConnections.Add(i, tmp)


            //Next

            //For i As Int32 = 0 To NumberofConnections
            //    Dim queuemanager As String = My.Settings.QueueManager
            //    Dim channel As String = My.Settings.channelName
            //    Dim connection As String = My.Settings.connectionName
            //    Dim channelID As String = My.Settings.ChannelID
            //    Dim queue As String = ReqQueus.brkXferTranAdd_Req
            //    Dim queuereply As String = ReplyQueus.brkXferTranAdd_Rply
            //    Dim req As New QueueINFO
            //    req.channelName = channel
            //    req.connectionName = connection
            //    req.QueueManager = queuemanager
            //    req.QueueName = queue
            //    req.QueueNameReply = queuereply

            //    Dim tmp As New ZagQueue(req)

            //    HsAddTransactionsOfConnections.Add(i, tmp)



            //Next

            //For i As Int32 = 0 To NumberofConnections
            //    Dim queuemanager As String = My.Settings.QueueManager
            //    Dim channel As String = My.Settings.channelName
            //    Dim connection As String = My.Settings.connectionName
            //    Dim channelID As String = My.Settings.ChannelID
            //    Dim queue As String = ReqQueus.brkAddLien_Req
            //    Dim queuereply As String = ReplyQueus.brkAddLien_Rply
            //    Dim req As New QueueINFO
            //    req.channelName = channel
            //    req.connectionName = connection
            //    req.QueueManager = queuemanager
            //    req.QueueName = queue
            //    req.QueueNameReply = queuereply

            //    Dim tmp As New ZagQueue(req)

            //    HsAddLianOfConnections.Add(i, tmp)



            //Next

            //For i As Int32 = 0 To NumberofConnections
            //    Dim queuemanager As String = My.Settings.QueueManager
            //    Dim channel As String = My.Settings.channelName
            //    Dim connection As String = My.Settings.connectionName
            //    Dim channelID As String = My.Settings.ChannelID
            //    Dim queue As String = ReqQueus.brkAcctLienMod_Req
            //    Dim queuereply As String = ReplyQueus.brkAcctLienMod_Rply
            //    Dim req As New QueueINFO
            //    req.channelName = channel
            //    req.connectionName = connection
            //    req.QueueManager = queuemanager
            //    req.QueueName = queue
            //    req.QueueNameReply = queuereply

            //    Dim tmp As New ZagQueue(req)

            //    HsModifyOfConnections.Add(i, tmp)



            //Next

            //Dim mycallback As New System.Threading.TimerCallback(AddressOf ReOpenClosedQueus)
            //MyTimer = New System.Threading.Timer(mycallback, Nothing, 500, 500)


        }


    }



    public class QueueINFO
    {
        public string QueueName;
        public string QueueNameReply;

        public string QueueManager;
        public string channelName;
        public string connectionName;


    }

    public struct ReqQueus
    {

        public static System.String brkGetLastNTransactions_Req = "brkGetLastNTransactions_Req";
        public static System.String brkAcctLienMod_Req = "brkAcctLienMod_Req";
        public static System.String brkAddLien_Req = "brkAddLien_Req";
        public static System.String brkBalInq_Req = "brkBalInq_Req";
        public static System.String brkXferTranAdd_Req = "brkXferTranAdd_Req";


    }


    public struct ReplyQueus
    {
        public static System.String brkAcctLienMod_Rply = "brkAcctLienMod_Rply";
        public static System.String brkAddLien_Rply = "brkAddLien_Rply";
        public static System.String brkBalInq_Rply = "brkBalInq_Rply";
        public static System.String brkGetLastNTransactions_Rply = "brkGetLastNTransactions_Rply";
        public static System.String brkXferTranAdd_Rply = "brkXferTranAdd_Rply";

    }

    public class ZagQueue
    {
        public MQQueueManager mqQMgr;
        // MQQueueManager instance
        public MQQueue mqQueue;
        // MQQueue instance
        public MQMessage mqMsg;
        MQMessage mqMsgOut;
        int tmpVar;

        // MQMessage instance
        public MQPutMessageOptions mqPutMsgOpts;
        public MQGetMessageOptions mqGetMsgOpts;
        // MQPutMessageOptions instance
        public string queueName;
        public bool IsAvialable;
        public MQQueue queuIn;
        private QueueINFO _RequestInfo;
        public ZagQueue(QueueINFO RequestInfo)
        {
            // Name of queue to use
            _RequestInfo = RequestInfo;

            if (string.IsNullOrEmpty(RequestInfo.QueueManager))
            {
                // MsgBox("Usage: SendToWebSphere <queue> <xml_Filename>")
                IsAvialable = false;
            }
            else
            {
                queueName = RequestInfo.QueueName;
            }


            string QueueManager = RequestInfo.QueueManager;
            string channelName = RequestInfo.channelName;
            string connectionName = RequestInfo.connectionName;
            try
            {


                mqQMgr = new MQQueueManager(QueueManager, channelName, connectionName);
            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("Issue In : " + QueueManager + " | Channel Name : " + channelName + " | Connection Name:" + connectionName);
                Console.WriteLine("create of MQQueueManager ended with " + mqe.ToString());
                IsAvialable = false;
            }

            //
            // Try to open the queue
            //
            try
            {
                // open queue for output
                // but not if MQM stopping

                mqQueue = mqQMgr.AccessQueue(queueName, IBM.WMQ.MQC.MQOO_OUTPUT + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING);
                queuIn = mqQMgr.AccessQueue(RequestInfo.QueueNameReply, IBM.WMQ.MQC.MQOO_INPUT_SHARED + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "");
                IsAvialable = true;

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                IsAvialable = false;
            }


        }

        public void Reopen()
        {
            try
            {
                // open queue for output
                // but not if MQM stopping
                string QueueManager = _RequestInfo.QueueManager;
                string channelName = _RequestInfo.channelName;
                string connectionName = _RequestInfo.connectionName;
                mqQMgr = new MQQueueManager(QueueManager, channelName, connectionName);

                mqQueue = mqQMgr.AccessQueue(queueName, IBM.WMQ.MQC.MQOO_OUTPUT + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING);
                queuIn = mqQMgr.AccessQueue(_RequestInfo.QueueNameReply, IBM.WMQ.MQC.MQOO_INPUT_SHARED + IBM.WMQ.MQC.MQOO_FAIL_IF_QUIESCING, "", "QRAAMQ1", "");
                IsAvialable = true;

            }
            catch (MQException mqe)
            {
                // stop if failed
                Console.WriteLine("MQQueueManager::AccessQueue ended with " + mqe.ToString());
                IsAvialable = false;
            }


        }
    }
}
