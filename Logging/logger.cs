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

using System.Text;
using log4net;
using log4net.Config;
using ZagAPIServer;

namespace ZagAPIServer
{
	
	public enum LogLevel : int
	{
		DEBUG = 0,
		INFO = 1,
		WARN = 2,
		@ERROR = 3,
		FATAL = 4
	}
	
	public enum LogModes : int
	{
		APP = 0,
		CONN = 1,
		MKT = 2
	}
	
	public class Logger
	{
		public ILog LogApp;
		public ILog LogConn;
		public ILog LogCosole;
		public ILog LogMKT;
		public ILog LogCoreS;
		public ILog LogCoreR;
		public ILog LogHTP;
		public ILog LogAccountINQ;
		public ILog LogADDLean;
		public ILog LogRemoveLEAN;
		public ILog LogTransaction;
		public ILog LogAccountINQR;
		public ILog LogADDLeanR;
		public ILog LogRemoveLEANR;
		public ILog LogTransactionR;
		public ILog LogVoice;
		
		
		public void IntiApplog()
		{
			
			string strLogFileName = string.Empty;
			log4net.Config.XmlConfigurator.Configure();
			LogApp = LogManager.GetLogger("APP2");
			LogCoreR = LogManager.GetLogger("Corer");
			LogCoreS = LogManager.GetLogger("Cores");
			LogHTP = LogManager.GetLogger("HTP");
			LogAccountINQ = LogManager.GetLogger("ACCINQL");
			LogAccountINQR = LogManager.GetLogger("ACCINQLR");
			LogADDLean = LogManager.GetLogger("ADDLEANL");
			LogADDLeanR = LogManager.GetLogger("ADDLEANLR");
			LogRemoveLEAN = LogManager.GetLogger("REMLEANL");
			LogRemoveLEANR = LogManager.GetLogger("REMLEANLR");
			LogTransaction = LogManager.GetLogger("TransL");
			LogTransactionR = LogManager.GetLogger("TransLR");
			LogVoice = LogManager.GetLogger("Voice");
			
			
			// LogCosole = LogManager.GetLogger("Console")
			// LogMKT = LogManager.GetLogger("MKTLOG")
			
		}
		public void IntiConnlog()
		{
			
			//Dim strLogFileName As String = [String].Empty
			//strLogFileName = "Connections.log"
			//log4net.GlobalContext.Properties("LogFileName") = strLogFileName
			//log4net.Config.XmlConfigurator.Configure()
			LogConn = LogManager.GetLogger("CONN2");
			
		}
		
		public void LogMsg(LogModes mode, LogLevel level, object msg)
		{
			//  Console.WriteLine(msg)
			// log.LogCosole.Info(msg)
			
			
			// SyncLock msg
			switch (mode)
			{
				case LogModes.APP:
					switch (level)
					{
						case LogLevel.DEBUG:
							LogApp.Debug(msg);
							break;
							
						case LogLevel.INFO:
							LogApp.Info(msg);
							break;
							
						case LogLevel.WARN:
							LogApp.Warn(msg);
							break;
							
						case LogLevel.ERROR:
							LogApp.Error(msg);
							break;
							
						case LogLevel.FATAL:
							LogApp.Fatal(msg);
							break;
					}
					break;
				case LogModes.CONN:
					switch (level)
					{
						case LogLevel.DEBUG:
							LogConn.Debug(msg);
							break;
							
						case LogLevel.INFO:
							LogConn.Info(msg);
							break;
							
						case LogLevel.WARN:
							LogConn.Warn(msg);
							break;
							
						case LogLevel.ERROR:
							LogConn.Error(msg);
							break;
							
						case LogLevel.FATAL:
							LogConn.Fatal(msg);
							break;
					}
					break;
					
				case LogModes.MKT:
					switch (level)
					{
						case LogLevel.DEBUG:
							LogMKT.Debug(msg);
							break;
							
						case LogLevel.INFO:
							LogMKT.Info(msg);
							break;
							
						case LogLevel.WARN:
							LogMKT.Warn(msg);
							break;
							
						case LogLevel.ERROR:
							LogMKT.Error(msg);
							break;
							
						case LogLevel.FATAL:
							LogMKT.Fatal(msg);
							break;
					}
					break;
					
					
			}
			
			
			//End SyncLock
			
		}
	}
	
	
}
