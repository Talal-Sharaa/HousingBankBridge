// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.IO;
using System.Xml;
using VDS;
using VDS.Web;
using VDS.Web.Handlers;
using VDS.Web.Modules;
using ZagAPIServer;

namespace ZagAPIServer
{
	
	public class HousingBankCoreBankSimulatorHandler : VDS.Web.Handlers.IHttpListenerHandler
	{
		private string _HandlerType = "Housing Bank Simulator";
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
		
		void IHttpListenerHandler.ProcessRequest(HttpServerContext context)
		{
			this.ProcessRequestAsync(context);
		}
		
		public void ProcessRequestAsync(HttpServerContext context)
		{
			Console.WriteLine(context.Request.ToString());
			string str = context.Request.Url.LocalPath.Replace("/", "");
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
				
				if (ArrOfCommands.Contains(cmd) == false)
				{
					SendCustomErr(context, "Wrong Command");
					return;
				}
				
				context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8");
				
				switch (cmd)
				{
					case "GetUserBalance":
						ValidateInputsForUserBalance(context);
						break;
					case "GetUserTransactions":
						ValidateInputsForUserTransactions(context);
						break;
					case "AddUserTransaction":
						ValidateInputsForADDUserTransactions(context);
						break;
					case "SetUserReservation":
						ValidateInputsForSetUserReservation(context);
						break;
					case "DelUserReservation":
						ValidateInputsForDelUserReservation(context);
						break;
					case "GetUserReservations":
						ValidateInputsForGetUserReservations(context);
						break;



				}

			}
			else
			{
				context.Response.StatusCode = 404;
			}
			
			
			
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
		public void ValidateInputsForUserBalance(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			string BranchID = Context.Request.QueryString["BranchID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
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
			//StreamWriter writer = new StreamWriter(context.Response.OutputStream)
			VBMath.Randomize();
			
			int i;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 4);
			
			string filename = "ResultsBal\\" + AccountUserID +"." + BranchID +".xml";
			if (System.IO.File.Exists(filename))
			{
				conetext.Response.AddHeader("Content-Type", "text/xml; charset=utf-8");
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(conetext.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(conetext, "Account Not Found");
				
			}
			
			
		}
#endregion
		
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
			
			//HandelGetUserTransActions(Context, AccountUserID, BranchID, NumberOfTransActions)
			VBMath.Randomize();
			
			int i = 0;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 1);
			
			string filename = "getLastTransactions\\" + System.Convert.ToString(i) +".xml";
			if (System.IO.File.Exists(filename))
			{
				// Context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8")
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(Context, "Account Not Found");
				
			}
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		public void ValidateInputsForADDUserTransactions(HttpServerContext Context)
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
			if (string.IsNullOrWhiteSpace(Currency))
			{
				SendCustomErr(Context, "Reason parameter must be passed");
				return;
			}
			
			
			VBMath.Randomize();
			
			int i = 0;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 1);
			
			string filename = "AddTrx\\" + System.Convert.ToString(i) +".xml";
			if (System.IO.File.Exists(filename))
			{
				// Context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8")
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(Context, "Account Not Found");
				
			}
			
			// HandelADDUserTransActions(Context, AccountUserID, BranchID, Amount, CD, Currency, Reason)
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		
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
			
			VBMath.Randomize();
			
			int i = 0;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 3);
			
			string filename = "SetRes\\" + System.Convert.ToString(i) +".xml";
			if (System.IO.File.Exists(filename))
			{
				// Context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8")
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(Context, "Account Not Found");
				
			}
			
			
			
			// HandelSetUserReservation(Context, AccountUserID, Amount, Currency, ReasonCode, IsFirst)
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
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
			
			
			VBMath.Randomize();
			
			int i = 0;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 3);
			
			string filename = "DelRes\\" + System.Convert.ToString(i) +".xml";
			if (System.IO.File.Exists(filename))
			{
				// Context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8")
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(Context, "Account Not Found");
				
			}
			
			// HandelDelUserReservation(Context, AccountUserID, LIENID, Currency, ReasonCode)
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
		
		public void ValidateInputsForGetUserReservations(HttpServerContext Context)
		{
			string AccountUserID = Context.Request.QueryString["AccountUserID"];
			if (string.IsNullOrWhiteSpace(AccountUserID))
			{
				SendCustomErr(Context, "AccountUserID parameter must be passed");
				return;
			}
			
			
			VBMath.Randomize();
			
			int i = 0;
			i = System.Convert.ToInt32(1 + VBMath.Rnd() * 1);
			
			string filename = "GetAllReservations\\" + System.Convert.ToString(i) +".xml";
			if (System.IO.File.Exists(filename))
			{
				// Context.Response.AddHeader("Content-Type", "text/xml; charset=utf-8")
				var str = System.IO.File.ReadAllText(filename);
				StreamWriter writer = new StreamWriter(Context.Response.OutputStream);
				writer.Write(str);
				writer.Close();
			}
			else
			{
				SendCustomErr(Context, "Account Not Found");
				
			}
			
			
			//ZAGRSNCODE
			
			
			//If IsNumeric(Amount) = False Then
			//    SendCustomErr(Context, "Amount parameter must be Numric")
			//    Exit Sub
			//End If
			
			//CURRENCY
			
			
			
			//  HandelGetUserReservations(Context, AccountUserID)
			
			
			
			
			//  Dim ArrOfCommands As New Collection
			
		}
		
	}
	
}
