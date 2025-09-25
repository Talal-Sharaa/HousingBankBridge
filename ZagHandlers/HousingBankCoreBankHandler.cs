// VBConversions Note: VB project level imports
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
using VDS;
using VDS.Web;
using VDS.Web.Handlers;
using VDS.Web.Modules;
using ZagAPIServer;
using ZagAPIServer.Properties;
using ZagAPIServer.Tools;

namespace ZagAPIServer
{
	public class HousingBankCoreBankHandler : VDS.Web.Handlers.IHttpListenerHandler
	{
		private string _HandlerType = "Housing Bank IBM Websphare MQ";
		
		public HousingBankCoreBankHandler()
		{
			
		}
		
		public string HandlerType
		{
			get
			{
				return _HandlerType;
			}
			
		}
		
		public bool IsReusable
		{
			get
			{
				return true;
				
			}
		}
		
		async void IHttpListenerHandler.ProcessRequest(HttpServerContext context)
		{
			this.ProcessRequestAsync(context);
		}
		
		public async void ProcessRequestAsync(HttpServerContext context)
		{
			try
			{
				string str = context.Request.Url.LocalPath.Replace("/", "");
				PublicInfo.log.LogHTP.Info(DateTime.Now + " : Incoming Request :" + context.Request.Url.ToString());
				
				// Await    Task.Factory.StartNew (  Sub ()
				if (str.ToLower() == "ExECommand.Zag".ToLower())
				{
					
					
					string cmd = context.Request.QueryString["Cmd"];
					if (string.IsNullOrWhiteSpace(cmd))
					{
						SendCustomErr(context, "Cmd parameter must be passed");
						return;
					}
					Collection ArrOfCommands = new Collection();
					
					
					ArrOfCommands.Add("GetUserBalance", "GetUserBalance", null, null);
					ArrOfCommands.Add("DelUserReservation", "DelUserReservation", null, null);
					ArrOfCommands.Add("GetUserTransactions", "GetUserTransactions", null, null);
					ArrOfCommands.Add("SetUserReservation", "SetUserReservation", null, null);
					ArrOfCommands.Add("AddUserTransaction", "AddUserTransaction", null, null);
					ArrOfCommands.Add("GetUserReservations", "GetUserReservations", null, null);
					ArrOfCommands.Add("GetUserStatus", "GetUserStatus", null, null);
					ArrOfCommands.Add("GetMQStatus", "GetMQStatus", null, null);
					
					if (ArrOfCommands.Contains(cmd) == false)
					{
						SendCustomErr(context, "Wrong Command");
						return;
					}
					
					context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8");
					cmd = cmd.ToUpper();
					Console.WriteLine(DateTime.Now + " : New Call For  : " + cmd + " :" + context.Request.RawUrl);
					if (cmd == "GetUserBalance".ToUpper())
					{
						
						ValidateInputsForUserBalance(context);
					}
					else if (cmd == "GetUserTransactions".ToUpper())
					{
						ValidateInputsForUserTransactions(context);
					}
					else if (cmd == "AddUserTransaction".ToUpper())
					{
						ValidateInputsForADDUserTransactions(context);
					}
					else if (cmd == "SetUserReservation".ToUpper())
					{
						ValidateInputsForSetUserReservation(context);
					}
					else if (cmd == "DelUserReservation".ToUpper())
					{
						ValidateInputsForDelUserReservation(context);
					}
					else if (cmd == "GetUserReservations".ToUpper())
					{
						ValidateInputsForGetUserReservations(context);
					}
					else if (cmd == "GetUserStatus".ToUpper())
					{
						ValidateInputsForUserStatus(context);
					}
					else if (cmd == "GetMQStatus".ToUpper())
					{
						ValidateInputsForMQStatus(context);
					}
					
					
					
				}
				else
				{
					context.Response.StatusCode = 404;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("general exception  : " + ex.ToString());
			}
			
			//End Sub )
			
			
		}
		
		private void ValidateInputsForMQStatus(HttpServerContext context)
		{
			HandelUserMQ(context);
			
		}
		public void HandelUserMQ(HttpServerContext Context)
		{
			VBMath.Randomize();
			Context.Response.Headers.Clear();
			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkBalInq_Req;
			string queuereply = ReplyQueus.brkBalInq_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			string requestID = "Req_Zag_Balnce_" + DateTime.Now.ToOADate().ToString().Replace(".", "") + System.Convert.ToString(50 + VBMath.Rnd() * 9000);
			
			string requetdata = Resource1.AccINQ;
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_X", id);
			DateTime currentDate = new DateTime(); // (2018, 4, 8, 4, 2, 1)
			currentDate = DateTime.Now;
			
			
			requetdata = requetdata.Replace("ZAGACC", "0000011111");
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			
			//requetdata = requetdata.Replace("ZAGBID", BranchID)
			requetdata = requetdata.Replace("BRK", channelID);
			// log.LogAccountINQ.Info("Sending To Core :" & requetdata)
			string str = "";
			
			try
			{
				str = ZAGIBMMQ.Instance.GetDataFromIBMMQDirectForTestConnection(req, requetdata); //(req, requetdata)
			}
			catch (Exception ex)
			{
				str = ex.ToString();
			}
			
			
			//  log.LogAccountINQR.Info("Response From Core : " & str)
			//Dim strr As New StringReader(str)
			//Dim ds As New DataSet()
			//ds.ReadXml(strr)
			//Dim tmp As String = ds.Tables("AcctInqRs").Rows(0)("BankAcctStatusCode")
			
			
			//ds.Tables("AcctInqRs").Rows(0)("BankAcctStatusCode")
			StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
			writer.Write(str);
			writer.Close();
			
		}
		
		public void SendCustomErr(HttpServerContext context, string ErrMsg)
		{
			context.Response.StatusCode = 400;
			var doc = new XmlDocument();
			context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8");
			XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
			XmlElement root = doc.DocumentElement;
			doc.InsertBefore(xmlDeclaration, root);
			
			//(2) string.Empty makes cleaner code
			XmlElement element1 = doc.CreateElement(string.Empty, "ErrorBody", string.Empty);
			doc.AppendChild(element1);
			
			XmlElement element2 = doc.CreateElement(string.Empty, "ErrorMessage", string.Empty);
			element1.AppendChild(element2);
			
			//Dim element3 As XmlElement = doc.CreateElement(String.Empty, "level2", String.Empty)
			XmlText text1 = doc.CreateTextNode(ErrMsg);
			element2.AppendChild(text1);
			// element2.AppendChild(element3)
			
			
			StreamWriter writer = new StreamWriter(context.Response.OutputStream);
			writer.Write(doc.InnerXml);
			writer.Close();
			
		}
#region User Balanace Section
		public void ValidateInputsForUserStatus(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			
			
			HandelGetUserStatus(Context, AccountUserID);
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		public void HandelGetUserStatus(HttpServerContext conetext, string AccountUserID)
		{
			VBMath.Randomize();
			// conetext.Response.Headers.Clear()
			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkBalInq_Req;
			string queuereply = ReplyQueus.brkBalInq_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			string requestID = "Req_Zag_Balnce_" + DateTime.Now.ToOADate().ToString().Replace(".", "") + System.Convert.ToString(50 + VBMath.Rnd() * 9000);
			
			string requetdata = Resource1.AccINQ;
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_X", id);
			DateTime currentDate = new DateTime(); // (2018, 4, 8, 4, 2, 1)
			currentDate = DateTime.Now;
			
			
			requetdata = requetdata.Replace("ZAGACC", AccountUserID);
			//requetdata = requetdata.Replace("ZAGBID", BranchID)
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			
			requetdata = requetdata.Replace("BRK", channelID);
			PublicInfo.log.LogAccountINQ.Info("Sending To Core :" + requetdata);
			
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQForBallance(req, requetdata);
			if(ConfigurationManager.AppSettings["MaskCustIdAndChildren"].ToLower() == "true")
			{
				str = Tool.MaskCustIdAndChildren(str);
			}
			PublicInfo.log.LogAccountINQR.Info("Response From Core : " + str);
			//Dim strr As New StringReader(str)
			//Dim ds As New DataSet()
			//ds.ReadXml(strr)
			//Dim tmp As String = ds.Tables("AcctInqRs").Rows(0)("BankAcctStatusCode")
			
			
			//ds.Tables("AcctInqRs").Rows(0)("BankAcctStatusCode")
			try
			{
				StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			catch (Exception)
			{
				Console.WriteLine("Caller Canceled the Request");
			}
			
		}
		public void ValidateInputsForUserBalance(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			string BranchID = Context.Request.QueryString["BranchID"];
			if (string.IsNullOrWhiteSpace(BranchID))
			{
				SendCustomErr(Context, "BranchID parameter must be passed");
				return;
			}
			if (Information.IsNumeric(BranchID) == false)
			{
				SendCustomErr(Context, "BranchID parameter must be Numric");
				return;
			}
			
			HandelGetUserBalance(Context, AccountUserID, BranchID);
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		public void HandelGetUserBalance(HttpServerContext conetext, string AccountUserID, string BranchID)
		{
			VBMath.Randomize();
			
			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkBalInq_Req;
			string queuereply = ReplyQueus.brkBalInq_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			string requestID = "Req_Zag_Balnce_" + DateTime.Now.ToOADate().ToString().Replace(".", "") + System.Convert.ToString(50 + VBMath.Rnd() * 9000);
			
			string requetdata = Resource1.BalInc;
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_X", id);
			
			requetdata = requetdata.Replace("ZAGACCID", AccountUserID);
			requetdata = requetdata.Replace("ZAGBID", BranchID);
			requetdata = requetdata.Replace("SMS", channelID);
			PublicInfo.log.LogAccountINQ.Info("Sending To Core :" + requetdata);
			
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQForBallance(req, requetdata);
			
			PublicInfo.log.LogAccountINQR.Info("Response From Core : " + str);
			
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
		
		
		
		
#endregion
		
#region GetUserTransactions
		
		public void ValidateInputsForUserTransactions(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			string BranchID = Context.Request.QueryString["BranchID"];
			if (string.IsNullOrWhiteSpace(BranchID))
			{
				SendCustomErr(Context, "BranchID parameter must be passed");
				return;
			}
			if (Information.IsNumeric(BranchID) == false)
			{
				SendCustomErr(Context, "BranchID parameter must be Numric");
				return;
			}
			
			string NumberOfTransActions = Context.Request.QueryString["NumberOfTransactions"];
			if (string.IsNullOrWhiteSpace(NumberOfTransActions))
			{
				SendCustomErr(Context, "NumberOfTransactions parameter must be passed");
				return;
			}
			
			if (Information.IsNumeric(NumberOfTransActions) == false)
			{
				SendCustomErr(Context, "NumberOfTransactions parameter must be Numric");
				return;
			}
			
			HandelGetUserTransActions(Context, AccountUserID, BranchID, NumberOfTransActions);
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void HandelGetUserTransActions(HttpServerContext conetext, string AccountUserID, string BranchID, string NumberOfTransaction)
		{
			VBMath.Randomize();

			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkGetLastNTransactions_Req;
			string queuereply = ReplyQueus.brkGetLastNTransactions_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			// Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)
			
			string requetdata = Resource1.LastTransactions;
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			id = System.Convert.ToString(("Req_Trans_" + System.Convert.ToString(DateTime.Now.ToOADate())).Replace(".", ""));
			requetdata = requetdata.Replace("Req_Zag_Trans_X", id);
			DateTime currentDate = new DateTime(2018, 4, 8, 4, 2, 1);
			currentDate = DateTime.Now;
			requetdata = requetdata.Replace("ZAGACCID", AccountUserID);
			requetdata = requetdata.Replace("ZAGBID", BranchID);
			requetdata = requetdata.Replace("SMS", channelID);
			requetdata = requetdata.Replace("ZAGNUMBEROFTRANSACTIONS", NumberOfTransaction);
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			PublicInfo.log.LogCoreS.Info("Sending To Core :" + requetdata);
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQForGetTransactions(req, requetdata);
			PublicInfo.log.LogCoreR.Info("Response From Core : " + str);
			// log.LogCoreS.Info("Sending To Core :" & requetdata)
			
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
#endregion
		
#region AddUserTransactions
		
		public void ValidateInputsForADDUserTransactions(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			string BranchID = ""; // Context.Request.QueryString("BranchID")
			//If String.IsNullOrWhiteSpace(BranchID) Then
			//    SendCustomErr(Context, "BranchID parameter must be passed")
			//    Exit Sub
			//End If
			//If IsNumeric(BranchID) = False Then
			//    SendCustomErr(Context, "BranchID parameter must be Numric")
			//    Exit Sub
			//End If
			
			string Amount = Context.Request.QueryString["Amounts"];
			if (string.IsNullOrWhiteSpace(Amount))
			{
				SendCustomErr(Context, "Amount parameter must be passed");
				return;
			}
			
			//If IsNumeric(Amount) = False Then
			//    SendCustomErr(Context, "Amount parameter must be Numric")
			//    Exit Sub
			//End If
			string CD = Context.Request.QueryString["CD"];
			if (string.IsNullOrWhiteSpace(CD))
			{
				SendCustomErr(Context, "CD parameter must be passed");
				return;
			}
			//CURRENCY
			string Currency = Context.Request.QueryString["Currency"];
			if (string.IsNullOrWhiteSpace(Currency))
			{
				SendCustomErr(Context, "Currency parameter must be passed");
				return;
			}
			
			string Reason = Context.Request.QueryString["Reason"];
			if (string.IsNullOrWhiteSpace(Reason))
			{
				SendCustomErr(Context, "Reason parameter must be passed");
				return;
			}
			
			
			HandelADDUserTransActions(Context, AccountUserID, BranchID, Amount, CD, Currency, Reason);
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void HandelADDUserTransActions(HttpServerContext conetext, string AccountUserID, string BranchID, string Amount, string CD, string Currency, string Reason)
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
			//Req_Zag_Balnce_X
			// Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)
			
			string requetdata = Resource1.SendTransActions;
			// Dim id As String
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_ADD_Trans_X", id);
			//Req_Zag_ADD_Trans_X
			
			string[] arrofAccounts = AccountUserID.Split(',');
			string[] arrofammounts = Amount.Split(',');
			string[] arrofcd = CD.Split(',');
			string[] arrofReasons = Reason.Split(',');
			
			if (arrofAccounts.Length != arrofammounts.Length | arrofammounts.Length != arrofcd.Length | arrofAccounts.Length != arrofReasons.Length)
			{
				SendCustomErr(conetext, "Parameters Numbers Not Match");
				
				
			}
			
                        StringBuilder strLegs = new StringBuilder();
                        HashSet<string> uniqueLegs = new HashSet<string>(StringComparer.Ordinal);
                        int k = 1;

                        for (int i = 0; i <= arrofcd.Length - 1; i++)
                        {
                                string currentFlag = arrofcd[i].Trim();
                                string normalizedFlag = currentFlag.ToUpperInvariant();
                                string amount = arrofammounts[i].Trim();
                                string valueDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff");

                                string tmp = Resource1.TransLeg;
                                tmp = tmp.Replace("ZAGACCID", arrofAccounts[i].Trim());
                                tmp = tmp.Replace("ZAGFLAG", currentFlag);
                                tmp = tmp.Replace("ZAGAMOUNT", amount);
                                tmp = tmp.Replace("ZAGCURRENCY", Currency);
                                tmp = tmp.Replace("ZAGDATE", valueDate);
                                string legKey = tmp.Replace(valueDate, string.Empty);
                                if (uniqueLegs.Add(legKey))
                                {
                                        string number = Strings.Format(k, "00000");
                                        string finalLeg = tmp.Replace("ZAGSERIAL", number);
                                        strLegs.Append(finalLeg + Constants.vbNewLine);
                                        k++;
                                }

                                tmp = Resource1.TransLeg;
                                tmp = tmp.Replace("ZAGACCID", System.Convert.ToString(ConfigurationManager.AppSettings["PoolAccountNumber"]));
                                tmp = tmp.Replace("ZAGFLAG", (normalizedFlag == "C") ? "D" : "C");
                                tmp = tmp.Replace("ZAGAMOUNT", amount);
                                tmp = tmp.Replace("ZAGCURRENCY", Currency);
                                tmp = tmp.Replace("ZAGDATE", valueDate);
                                legKey = tmp.Replace(valueDate, string.Empty);
                                if (uniqueLegs.Add(legKey))
                                {
                                        string number = Strings.Format(k, "00000");
                                        string finalLeg = tmp.Replace("ZAGSERIAL", number);
                                        strLegs.Append(finalLeg + Constants.vbNewLine);
                                        k++;
                                }

                        }
                        requetdata = requetdata.Replace("ZAGLEGS", strLegs.ToString());

                        k = 1;
                        StringBuilder strTails = new StringBuilder();
                        HashSet<string> uniqueTails = new HashSet<string>(StringComparer.Ordinal);
                        int num = System.Convert.ToInt32(10000000 + VBMath.Rnd() * 999999);

                        for (int i = 0; i <= arrofcd.Length - 1; i++)
                        {
                                string reasonCode = arrofReasons[i];
                                string tmp = Resource1.TransTail;
                                tmp = tmp.Replace("ZAGPURPOSCODE", reasonCode);
                                string timeStamp = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                                tmp = tmp.Replace("ZAGTIME", timeStamp);
                                tmp = tmp.Replace("ZAGID", System.Convert.ToString(num));
                                string tailKey = tmp.Replace(timeStamp, string.Empty);
                                if (uniqueTails.Add(tailKey))
                                {
                                        string number = Strings.Format(k, "00000");
                                        string finalTail = tmp.Replace("ZAGSERIAL", number);
                                        strTails.Append(finalTail + Constants.vbNewLine);
                                        k++;
                                }

                                tmp = Resource1.TransTail;
                                tmp = tmp.Replace("ZAGPURPOSCODE", reasonCode);
                                string poolTimeStamp = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                                tmp = tmp.Replace("ZAGTIME", poolTimeStamp);
                                tmp = tmp.Replace("ZAGID", System.Convert.ToString(num + 1));
                                tailKey = tmp.Replace(poolTimeStamp, string.Empty);
                                if (uniqueTails.Add(tailKey))
                                {
                                        string number = Strings.Format(k, "00000");
                                        string finalTail = tmp.Replace("ZAGSERIAL", number);
                                        strTails.Append(finalTail + Constants.vbNewLine);
                                        k++;
                                }

                        }

                        requetdata = requetdata.Replace("ZAGTAIL", strTails.ToString());
			requetdata = requetdata.Replace("ZAGDATE", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			requetdata = requetdata.Replace("SMS", channelID);
			
			//requetdata = File.ReadAllText("e:\tran_req.xml")
			//requetdata = requetdata.Replace("Zag", id)
			
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQDirect(req, requetdata);
			
			
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
#endregion
		
		
		
		
#region SetUserReservation
		
		public void ValidateInputsForSetUserReservation(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			//ZAGRSNCODE
			string ReasonCode = Context.Request.QueryString["ReasonCode"];
			if (string.IsNullOrWhiteSpace(ReasonCode))
			{
				SendCustomErr(Context, "Reason Code  parameter must be passed");
				return;
			}
			
			
			string Amount = Context.Request.QueryString["Amount"];
			if (string.IsNullOrWhiteSpace(Amount))
			{
				SendCustomErr(Context, "Amount parameter must be passed");
				return;
			}
			
			if (Information.IsNumeric(Amount) == false)
			{
				SendCustomErr(Context, "Amount parameter must be Numric");
				return;
			}
			
			//CURRENCY
			string Currency = Context.Request.QueryString["Currency"];
			if (string.IsNullOrWhiteSpace(Currency))
			{
				SendCustomErr(Context, "Currency parameter must be passed");
				return;
			}
			
			
			string IsFirst = Context.Request.QueryString["IsFirst"];
			if (string.IsNullOrWhiteSpace(Currency))
			{
				SendCustomErr(Context, "IsFirst parameter must be passed");
				return;
			}
			
			
			
			
			HandelSetUserReservation(Context, AccountUserID, Amount, Currency, ReasonCode, IsFirst);
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void HandelSetUserReservation(HttpServerContext conetext, string AccountUserID, string Amount, string Currency, string ReasonCode, string isFirst)
		{
			VBMath.Randomize();

			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);

			string queue = ReqQueus.brkAddLien_Req;
			string queuereply = ReplyQueus.brkAddLien_Rply;
			if (isFirst == "0")
			{
				queue = ReqQueus.brkAcctLienMod_Req;
				queuereply = ReplyQueus.brkAcctLienMod_Rply;
				
			}
			
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			// Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)
			DateTime currentDate = new DateTime(2018, 4, 8, 4, 2, 1);
			currentDate = DateTime.Now;
			string requetdata = Resource1.AddLIEN;
			if (isFirst == "0")
			{
				requetdata = Resource1.AddNextLien;
				
			}
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_Trans_X", id);
			requetdata = requetdata.Replace("Req_X", id);
			
			requetdata = requetdata.Replace("ZAGACCID", AccountUserID);
			requetdata = requetdata.Replace("ZAGRSNCODE", ReasonCode);
			requetdata = requetdata.Replace("SMS", channelID);
			// requetdata = requetdata.Replace("ZAGFLAG", CD)
			requetdata = requetdata.Replace("ZAGAMOUNT", Amount);
			requetdata = requetdata.Replace("ZAGCURRENCY", Currency);
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			// requetdata = requetdata.Replace("ZAGDATE", Now.AddSeconds(10).ToString("yyyy-MM-ddThh:mm:ss.fff"))
			//requetdata = requetdata.Replace("ZAGTIME", Now.ToString("yyyyMMddhhmmssfff"))
			PublicInfo.log.LogADDLean.Info("Sending To Core :" + requetdata);
			string str = "";
			if (isFirst == "1")
			{
				str = ZAGIBMMQ.Instance.GetDataFromIBMMQForAddLien(req, requetdata);
			}
			else
			{
				str = ZAGIBMMQ.Instance.GetDataFromIBMMQForModifyLien(req, requetdata);
			}
			
			
			PublicInfo.log.LogADDLeanR.Info("Response From Core : " + str);
			// log.LogCoreS.Info("Sending To Core :" & requetdata)
			
			
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
#endregion
		
		
#region DelUserReservation
		
		public void ValidateInputsForDelUserReservation(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			//ZAGRSNCODE
			string ReasonCode = Context.Request.QueryString["ReasonCode"];
			if (string.IsNullOrWhiteSpace(ReasonCode))
			{
				SendCustomErr(Context, "Reason Code  parameter must be passed");
				return;
			}
			
			
			string LIENID = Context.Request.QueryString["LIENID"];
			if (string.IsNullOrWhiteSpace(LIENID))
			{
				SendCustomErr(Context, "LIEN ID parameter must be passed");
				return;
			}
			
			//If IsNumeric(Amount) = False Then
			//    SendCustomErr(Context, "Amount parameter must be Numric")
			//    Exit Sub
			//End If
			
			//CURRENCY
			string Currency = Context.Request.QueryString["Currency"];
			if (string.IsNullOrWhiteSpace(Currency))
			{
				SendCustomErr(Context, "Currency parameter must be passed");
				return;
			}
			
			
			HandelDelUserReservation(Context, AccountUserID, LIENID, Currency, ReasonCode);
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void HandelDelUserReservation(HttpServerContext conetext, string AccountUserID, string LIENID, string Currency, string ReasonCode)
		{
			VBMath.Randomize();

			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkAcctLienMod_Req;
			string queuereply = ReplyQueus.brkAcctLienMod_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			// Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)
			DateTime currentDate = new DateTime(2018, 4, 8, 4, 2, 1);
			currentDate = DateTime.Now;
			string requetdata = Resource1.DelLIEN;
			// Dim id As String
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			requetdata = requetdata.Replace("Req_Zag_Trans_X", id);
			
			requetdata = requetdata.Replace("ZAGACCID", AccountUserID);
			requetdata = requetdata.Replace("ZAGRSNCODE", ReasonCode);
			requetdata = requetdata.Replace("SMS", channelID);
			// requetdata = requetdata.Replace("ZAGFLAG", CD)
			requetdata = requetdata.Replace("ZAGLIENID", LIENID);
			requetdata = requetdata.Replace("ZAGCURRENCY", Currency);
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			//requetdata = requetdata.Replace("ZAGTIME", Now.ToString("yyyyMMddhhmmssfff"))
			
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQForModifyLien(req, requetdata);
			
			PublicInfo.log.LogRemoveLEANR.Info(str);
			
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
#endregion
		
		
#region GetUserReservations
		
		public void ValidateInputsForGetUserReservations(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			//ZAGRSNCODE
			
			
			//If IsNumeric(Amount) = False Then
			//    SendCustomErr(Context, "Amount parameter must be Numric")
			//    Exit Sub
			//End If
			
			//CURRENCY
			
			
			
			HandelGetUserReservations(Context, AccountUserID);
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void HandelGetUserReservations(HttpServerContext conetext, string AccountUserID) // , LIENID As String, Currency As String, ReasonCode As String)
		{
			VBMath.Randomize();

			string queuemanager = System.Convert.ToString(ConfigurationManager.AppSettings["QueueManager"]);
			string channel = System.Convert.ToString(ConfigurationManager.AppSettings["channelName"]);
			string connection = System.Convert.ToString(ConfigurationManager.AppSettings["connectionName"]);
			string channelID = System.Convert.ToString(ConfigurationManager.AppSettings["ChannelID"]);
			string queue = ReqQueus.brkAcctLienMod_Req;
			string queuereply = ReplyQueus.brkAcctLienMod_Rply;
			QueueINFO req = new QueueINFO();
			req.channelName = channel;
			req.connectionName = connection;
			req.QueueManager = queuemanager;
			req.QueueName = queue;
			req.QueueNameReply = queuereply;
			//Req_Zag_Balnce_X
			// Dim requestID As String = "Req_Zag_Trans_" & Now.ToOADate.ToString().Replace(".", "") & (50 + Rnd() * 9000)
			DateTime currentDate = new DateTime(); // (2018, 4, 8, 4, 2, 1)
			currentDate = DateTime.Now;

			string requetdata = Resource1.GetAllLien;
			string id = "";
			id = "Req_X";
			
			
			id = System.Guid.NewGuid().ToString("N");
			id = id.Substring(0, 27);
			
			id = "Req_" + id;
			
			
			requetdata = requetdata.Replace("Req_X", id);
			
			requetdata = requetdata.Replace("ZAGACCID", AccountUserID);
			// requetdata = requetdata.Replace("ZAGRSNCODE", ReasonCode)
			requetdata = requetdata.Replace("SMS", channelID);
			// requetdata = requetdata.Replace("ZAGFLAG", CD)
			//'  requetdata = requetdata.Replace("ZAGLIENID", LIENID)
			//   requetdata = requetdata.Replace("ZAGCURRENCY", Currency)
			requetdata = requetdata.Replace("ZAGDATE", currentDate.ToString("yyyy-MM-ddThh:mm:ss.fff"));
			//requetdata = requetdata.Replace("ZAGTIME", Now.ToString("yyyyMMddhhmmssfff"))
			PublicInfo.log.LogAccountINQ.Info("Sending To Core :" + requetdata);
			string str = ZAGIBMMQ.Instance.GetDataFromIBMMQForModifyLien(req, requetdata);
			PublicInfo.log.LogAccountINQR.Info("Response From Core : " + str);
			// log.LogCoreS.Info("Sending To Core :" & requetdata)
			
			StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
			writer.Write(str);
			writer.Close();
		}
#endregion
		
	}
	
	
	
	
}
