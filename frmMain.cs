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

using System.ComponentModel;
using System.IO;
using VDS;
using VDS.Web;
using VDS.Web.Handlers;
using VDS.Web.Modules;
using VDS.Web.Logging;
using System.Runtime.InteropServices;
using ZagAPIServer;
using System.Configuration;

namespace ZagAPIServer
{
	
	
	public partial class frmMain
	{
		public frmMain()
		{
			InitializeComponent();
		}
		HttpServer ZagWebServer;
		bool ForceExit = false;
		[DllImport("kernel32.dll")]public static  extern bool AllocConsole();
		
		
		public void Button1_Click(object sender, EventArgs e)
		{
			// HttpListenerHandlerCollection handlers = new HttpListenerHandlerCollection()
			StartWebServer();
			
			
		}
		public void RestartReader()
		{
			// isRestartedFromMe = True
			
			ProcessStartInfo Info = new ProcessStartInfo();
			Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Application.ExecutablePath + "\"";
			Info.WindowStyle = ProcessWindowStyle.Hidden;
			Info.CreateNoWindow = true;
			Info.FileName = "cmd.exe";
			Process.Start(Info);
			Application.Exit();
		}
		
		public void StartWebServer()
		{
			try
			{
				HttpListenerHandlerCollection handlers = new HttpListenerHandlerCollection();
				handlers.AddMapping(new HttpRequestMapping(HttpRequestMapping.AllVerbs, "*.zag", (Type)cboHandlers.SelectedValue));
				ZagWebServer = new HttpServer("*", int.Parse(txtPortNum.Text), handlers);
				if (chkLogging.Checked)
				{
					FileLogger Logger = new FileLogger("ZagApiLogger.txt");
					ZagWebServer.AddLogger(Logger);
				}
				
				
				ZagWebServer.Start();
				NotifyIcon1.BalloonTipText = "Zag API web server started in port :" + txtPortNum.Text;
				NotifyIcon1.ShowBalloonTip(5000, "Started", "Zag API web server started in port :" + txtPortNum.Text, ToolTipIcon.Info);
				Button1.Text = "Server Started";
				Button1.Enabled = false;
				
			}
			catch (Exception ex)
			{
				NotifyIcon1.ShowBalloonTip(5000, "Error", ex.ToString(), ToolTipIcon.Error);
			}
			
			
		}
		
		public void Button2_Click(object sender, EventArgs e)
		{
			try
			{
				ZagWebServer.Stop();
				// ZagDeltaServer.Stop()
				NotifyIcon1.ShowBalloonTip(5000, "Stopped", "Zag API web server Stopped ", ToolTipIcon.Info);
			}
			catch (Exception)
			{
			}
			
			Button1.Text = "Start Server ";
			Button1.Enabled = true;
		}
		
		public void Form1_Closing(object sender, CancelEventArgs e)
		{
			SaveSettings();
			if (ForceExit == false)
			{
				e.Cancel = true;
				this.Visible = false;
				return;
				
			}
			
			if (ReferenceEquals(ZagWebServer, null) == false)
			{
				NotifyIcon1.Visible = false;
				Application.DoEvents();
				
				if (ZagWebServer.State == ServerState.Running)
				{
					ZagWebServer.Shutdown(true, true);
					
					Application.Exit();
					
					
					
				}
			}
			
			
		}

		public void LoadSetting()
		{
			PublicInfo.RegisterAllHandlers();

			txtPortNum.Text = ConfigurationManager.AppSettings["WebServerPortNumber"];
			chkAutoStart.Checked = bool.Parse(ConfigurationManager.AppSettings["AutoRunWebServer"]);
			BindingSource test = new BindingSource();
			test.DataSource = PublicInfo.AllhandlerColl;
			cboHandlers.DataSource = test;
			cboHandlers.DisplayMember = "Key";
			cboHandlers.ValueMember = "Value";
			cboHandlers.SelectedIndex = cboHandlers.FindStringExact(ConfigurationManager.AppSettings["DefWebHandler"]);
			chkLogging.Checked = bool.Parse(ConfigurationManager.AppSettings["EnableLogging"]);
			chkHide.Checked = bool.Parse(ConfigurationManager.AppSettings["HideMe"]);
			txtQmanager.Text = ConfigurationManager.AppSettings["QueueManager"];
			txtchannelName.Text = ConfigurationManager.AppSettings["channelName"];
			txtconnectionName.Text = ConfigurationManager.AppSettings["connectionName"];
			txtChannelID.Text = ConfigurationManager.AppSettings["ChannelID"];
			txtPreOpen.Text = ConfigurationManager.AppSettings["NumberOfPreOpenQ"];
			chkPreOpen.Checked = bool.Parse(ConfigurationManager.AppSettings["UsePreOpenQ"]);
			txtPoolaccount.Text = ConfigurationManager.AppSettings["PoolAccountNumber"];
			chkSync.Checked = bool.Parse(ConfigurationManager.AppSettings["SyncTransFromThisServer"]);
			txtNumberOfTRetries.Text = ConfigurationManager.AppSettings["NumberOfRetries"];
			chkMaskCustIdAndChildren.Checked = bool.Parse(ConfigurationManager.AppSettings["MaskCustIdAndChildren"]);
		}

		public void Form1_Shown(object sender, EventArgs e)
		{
			LoadSetting();
			AllocConsole();
			
			if (bool.Parse(ConfigurationManager.AppSettings["AutoRunWebServer"]))
			{
				StartWebServer();
				
			}
			if (bool.Parse(ConfigurationManager.AppSettings["HideMe"]))
			{
				this.Visible = false;
				
			}
			PublicInfo.log.IntiApplog();
			PublicInfo.log.LogCoreR.Info("Start App");
			try
			{
				if (chkSync.Checked)
				{
					PublicInfo.runtimer();
				}
				
			}
			catch (Exception)
			{
				
			}
			
			
		}
		public void SaveSettings()
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings["WebServerPortNumber"].Value = txtPortNum.Text;
			config.AppSettings.Settings["AutoRunWebServer"].Value = chkAutoStart.Checked.ToString();
			config.AppSettings.Settings["DefWebHandler"].Value = cboHandlers.Text;
			config.AppSettings.Settings["EnableLogging"].Value = chkLogging.Checked.ToString();
			config.AppSettings.Settings["HideMe"].Value = chkHide.Checked.ToString();
			config.AppSettings.Settings["QueueManager"].Value = txtQmanager.Text;
			config.AppSettings.Settings["channelName"].Value = txtchannelName.Text;
			config.AppSettings.Settings["connectionName"].Value = txtconnectionName.Text;
			config.AppSettings.Settings["ChannelID"].Value = txtChannelID.Text;
			config.AppSettings.Settings["NumberOfPreOpenQ"].Value = txtPreOpen.Text;
			config.AppSettings.Settings["UsePreOpenQ"].Value = chkPreOpen.Checked.ToString();
			config.AppSettings.Settings["PoolAccountNumber"].Value = txtPoolaccount.Text;
			config.AppSettings.Settings["NumberOfRetries"].Value = txtNumberOfTRetries.Text;
			config.AppSettings.Settings["MaskCustIdAndChildren"].Value = chkMaskCustIdAndChildren.Checked.ToString();
			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");


		}
		
		public void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Visible = true;
			
		}
		
		public void Form1_Load(object sender, EventArgs e)
		{
			
			
		}
		
		public void EndServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ForceExit = true;
			this.Close();
			
		}
		
		public void Button3_Click(object sender, EventArgs e)
		{
			SaveSettings();
		}
		
		private void Button4_Click(object sender, EventArgs e)
		{
			//Dim dt As Data = ParsXMLToDs(IO.File.ReadAllText("AddTrx\1.xml"))
			//MsgBox(dt.Rows(0)(1))
			
			//Dim dt2 As DataTable = ParsXMLToDs(IO.File.ReadAllText("AddTrx\2.xml"))
			//MsgBox(dt2.Rows(0)(1))
			
			
		}
		
		public void Button5_Click(object sender, EventArgs e)
		{
			PublicInfo.GetTransactionsToSyncFromZagDB();
			
		}
		
		public void GroupBox1_Enter(object sender, EventArgs e)
		{
			
		}
		
		public void Timer1_Tick(object sender, EventArgs e)
		{
			//Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
			//If (Now.Hour = 10 And Now.Minute = 3 And Now.Second < 3) Then
			//    Timer1.Enabled = False
			
			//    ' RestartReader()
			
			//End If
			//End Sub
		}
	}
	
}
