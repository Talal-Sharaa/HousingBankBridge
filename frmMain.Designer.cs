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

using ZagAPIServer;

namespace ZagAPIServer
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class frmMain : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)  {
			try
			{
				if (disposing && components != null)  {
						components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()  {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSync = new System.Windows.Forms.CheckBox();
            this.chkMaskCustIdAndChildren = new System.Windows.Forms.CheckBox();
            this.Button5 = new System.Windows.Forms.Button();
            this.chkHide = new System.Windows.Forms.CheckBox();
            this.chkLogging = new System.Windows.Forms.CheckBox();
            this.cboHandlers = new System.Windows.Forms.ComboBox();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtPortNum = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.txtNumberOfTRetries = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.txtPoolaccount = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.chkPreOpen = new System.Windows.Forms.CheckBox();
            this.txtPreOpen = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.txtChannelID = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Button3 = new System.Windows.Forms.Button();
            this.txtconnectionName = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtchannelName = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtQmanager = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.NotifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RestoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EndServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.ContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(492, 311);
            this.TabControl1.TabIndex = 1;
            // 
            // TabPage1
            // 
            this.TabPage1.Controls.Add(this.Button2);
            this.TabPage1.Controls.Add(this.Button1);
            this.TabPage1.Controls.Add(this.GroupBox1);
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(484, 285);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Main";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(256, 222);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(213, 40);
            this.Button2.TabIndex = 2;
            this.Button2.Text = "Stop";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(3, 222);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(213, 40);
            this.Button1.TabIndex = 1;
            this.Button1.Text = "Start";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.chkSync);
            this.GroupBox1.Controls.Add(this.Button5);
            this.GroupBox1.Controls.Add(this.chkHide);
            this.GroupBox1.Controls.Add(this.chkLogging);
            this.GroupBox1.Controls.Add(this.cboHandlers);
            this.GroupBox1.Controls.Add(this.chkAutoStart);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.txtPortNum);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.chkMaskCustIdAndChildren);
            this.GroupBox1.Location = new System.Drawing.Point(3, 17);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(466, 176);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Web Server Settings";
            this.GroupBox1.Enter += new System.EventHandler(this.GroupBox1_Enter);
            // 
            // chkSync
            // 
            this.chkSync.AutoSize = true;
            this.chkSync.Enabled = false;
            this.chkSync.Location = new System.Drawing.Point(187, 105);
            this.chkSync.Name = "chkSync";
            this.chkSync.Size = new System.Drawing.Size(197, 17);
            this.chkSync.TabIndex = 9;
            this.chkSync.Text = "Sync Transactions From This Server";
            this.chkSync.UseVisualStyleBackColor = true;
            //chkMaskCustIdAndChildren
            chkMaskCustIdAndChildren.Name = "chkMaskCustIdAndChildren";
            chkMaskCustIdAndChildren.Size = new System.Drawing.Size(200, 20);
            chkMaskCustIdAndChildren.Location = new System.Drawing.Point(187, 139); // Adjust location as needed
            this.chkMaskCustIdAndChildren.Text = "Mask Customer Info in Logs";
            this.chkMaskCustIdAndChildren.AutoSize = true;

            // 
            // Button5
            // 
            this.Button5.Location = new System.Drawing.Point(361, 19);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(93, 38);
            this.Button5.TabIndex = 8;
            this.Button5.Text = "Test Transaction";
            this.Button5.UseVisualStyleBackColor = true;
            this.Button5.Visible = false;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // chkHide
            // 
            this.chkHide.AutoSize = true;
            this.chkHide.Location = new System.Drawing.Point(9, 139);
            this.chkHide.Name = "chkHide";
            this.chkHide.Size = new System.Drawing.Size(151, 17);
            this.chkHide.TabIndex = 6;
            this.chkHide.Text = "Hide me when load Server";
            this.chkHide.UseVisualStyleBackColor = true;
            // 
            // chkLogging
            // 
            this.chkLogging.AutoSize = true;
            this.chkLogging.Location = new System.Drawing.Point(9, 105);
            this.chkLogging.Name = "chkLogging";
            this.chkLogging.Size = new System.Drawing.Size(100, 17);
            this.chkLogging.TabIndex = 5;
            this.chkLogging.Text = "Enable Logging";
            this.chkLogging.UseVisualStyleBackColor = true;
            // 
            // cboHandlers
            // 
            this.cboHandlers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboHandlers.FormattingEnabled = true;
            this.cboHandlers.Location = new System.Drawing.Point(123, 66);
            this.cboHandlers.Name = "cboHandlers";
            this.cboHandlers.Size = new System.Drawing.Size(226, 21);
            this.cboHandlers.TabIndex = 4;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(187, 31);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(168, 17);
            this.chkAutoStart.TabIndex = 3;
            this.chkAutoStart.Text = "Auto run web server at startup";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(6, 69);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(110, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "Default Web Handler:";
            // 
            // txtPortNum
            // 
            this.txtPortNum.Location = new System.Drawing.Point(123, 29);
            this.txtPortNum.Name = "txtPortNum";
            this.txtPortNum.Size = new System.Drawing.Size(58, 20);
            this.txtPortNum.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(6, 32);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(89, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Web Server Port:";
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.txtNumberOfTRetries);
            this.TabPage2.Controls.Add(this.Label9);
            this.TabPage2.Controls.Add(this.txtPoolaccount);
            this.TabPage2.Controls.Add(this.Label8);
            this.TabPage2.Controls.Add(this.chkPreOpen);
            this.TabPage2.Controls.Add(this.txtPreOpen);
            this.TabPage2.Controls.Add(this.Label7);
            this.TabPage2.Controls.Add(this.txtChannelID);
            this.TabPage2.Controls.Add(this.Label6);
            this.TabPage2.Controls.Add(this.Button3);
            this.TabPage2.Controls.Add(this.txtconnectionName);
            this.TabPage2.Controls.Add(this.Label5);
            this.TabPage2.Controls.Add(this.txtchannelName);
            this.TabPage2.Controls.Add(this.Label4);
            this.TabPage2.Controls.Add(this.txtQmanager);
            this.TabPage2.Controls.Add(this.Label3);
            this.TabPage2.Location = new System.Drawing.Point(4, 22);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(484, 285);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Housing Bank Setting";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // txtNumberOfTRetries
            // 
            this.txtNumberOfTRetries.Location = new System.Drawing.Point(195, 216);
            this.txtNumberOfTRetries.Name = "txtNumberOfTRetries";
            this.txtNumberOfTRetries.Size = new System.Drawing.Size(113, 20);
            this.txtNumberOfTRetries.TabIndex = 17;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(25, 219);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(164, 13);
            this.Label9.TabIndex = 16;
            this.Label9.Text = "Number Of Transactions Retries :";
            // 
            // txtPoolaccount
            // 
            this.txtPoolaccount.Location = new System.Drawing.Point(142, 190);
            this.txtPoolaccount.Name = "txtPoolaccount";
            this.txtPoolaccount.Size = new System.Drawing.Size(166, 20);
            this.txtPoolaccount.TabIndex = 15;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(25, 193);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(114, 13);
            this.Label8.TabIndex = 14;
            this.Label8.Text = "Pool Account Number:";
            // 
            // chkPreOpen
            // 
            this.chkPreOpen.AutoSize = true;
            this.chkPreOpen.Location = new System.Drawing.Point(328, 163);
            this.chkPreOpen.Name = "chkPreOpen";
            this.chkPreOpen.Size = new System.Drawing.Size(142, 17);
            this.chkPreOpen.TabIndex = 13;
            this.chkPreOpen.Text = "Enable Pre Open Queue";
            this.chkPreOpen.UseVisualStyleBackColor = true;
            // 
            // txtPreOpen
            // 
            this.txtPreOpen.Location = new System.Drawing.Point(142, 164);
            this.txtPreOpen.Name = "txtPreOpen";
            this.txtPreOpen.Size = new System.Drawing.Size(166, 20);
            this.txtPreOpen.TabIndex = 12;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(25, 167);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(105, 13);
            this.Label7.TabIndex = 11;
            this.Label7.Text = "# Pre Open Queues:";
            // 
            // txtChannelID
            // 
            this.txtChannelID.Location = new System.Drawing.Point(142, 108);
            this.txtChannelID.Name = "txtChannelID";
            this.txtChannelID.Size = new System.Drawing.Size(166, 20);
            this.txtChannelID.TabIndex = 10;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(25, 111);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(63, 13);
            this.Label6.TabIndex = 9;
            this.Label6.Text = "Channel ID:";
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(142, 251);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(166, 26);
            this.Button3.TabIndex = 8;
            this.Button3.Text = "Save";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // txtconnectionName
            // 
            this.txtconnectionName.Location = new System.Drawing.Point(142, 138);
            this.txtconnectionName.Name = "txtconnectionName";
            this.txtconnectionName.Size = new System.Drawing.Size(166, 20);
            this.txtconnectionName.TabIndex = 7;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(25, 141);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(95, 13);
            this.Label5.TabIndex = 6;
            this.Label5.Text = "Connection Name:";
            // 
            // txtchannelName
            // 
            this.txtchannelName.Location = new System.Drawing.Point(142, 78);
            this.txtchannelName.Name = "txtchannelName";
            this.txtchannelName.Size = new System.Drawing.Size(166, 20);
            this.txtchannelName.TabIndex = 5;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(25, 81);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(80, 13);
            this.Label4.TabIndex = 4;
            this.Label4.Text = "Channel Name:";
            // 
            // txtQmanager
            // 
            this.txtQmanager.Location = new System.Drawing.Point(142, 46);
            this.txtQmanager.Name = "txtQmanager";
            this.txtQmanager.Size = new System.Drawing.Size(166, 20);
            this.txtQmanager.TabIndex = 3;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(25, 49);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(87, 13);
            this.Label3.TabIndex = 2;
            this.Label3.Text = "Queue Manager:";
            // 
            // NotifyIcon1
            // 
            this.NotifyIcon1.ContextMenuStrip = this.ContextMenuStrip1;
            this.NotifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon1.Icon")));
            this.NotifyIcon1.Text = "Zag API web server";
            this.NotifyIcon1.Visible = true;
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RestoreToolStripMenuItem,
            this.EndServerToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            this.ContextMenuStrip1.Size = new System.Drawing.Size(130, 48);
            // 
            // RestoreToolStripMenuItem
            // 
            this.RestoreToolStripMenuItem.Name = "RestoreToolStripMenuItem";
            this.RestoreToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.RestoreToolStripMenuItem.Text = "Restore";
            this.RestoreToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // EndServerToolStripMenuItem
            // 
            this.EndServerToolStripMenuItem.Name = "EndServerToolStripMenuItem";
            this.EndServerToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.EndServerToolStripMenuItem.Text = "End Server";
            this.EndServerToolStripMenuItem.Click += new System.EventHandler(this.EndServerToolStripMenuItem_Click);
            // 
            // Timer1
            // 
            this.Timer1.Interval = 1000;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 311);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Zag API Server";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.ContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		
		internal TabControl TabControl1;
		internal TabPage TabPage1;
		internal Button Button1;
		internal GroupBox GroupBox1;
		internal TabPage TabPage2;
		internal Button Button2;
		internal TextBox txtPortNum;
		internal Label Label1;
		internal ComboBox cboHandlers;
		internal CheckBox chkAutoStart;
		internal Label Label2;
		internal CheckBox chkLogging;
		internal NotifyIcon NotifyIcon1;
		internal ContextMenuStrip ContextMenuStrip1;
		internal ToolStripMenuItem RestoreToolStripMenuItem;
		internal ToolStripMenuItem EndServerToolStripMenuItem;
		internal CheckBox chkHide;
		internal Button Button3;
		internal TextBox txtconnectionName;
		internal Label Label5;
		internal TextBox txtchannelName;
		internal Label Label4;
		internal TextBox txtQmanager;
		internal Label Label3;
		internal TextBox txtChannelID;
		internal Label Label6;
		internal CheckBox chkPreOpen;
		internal TextBox txtPreOpen;
		internal Label Label7;
		internal TextBox txtPoolaccount;
		internal Label Label8;
		internal Button Button5;
		internal CheckBox chkSync;
		internal CheckBox chkMaskCustIdAndChildren;
		internal TextBox txtNumberOfTRetries;
		internal Label Label9;
		internal Timer Timer1;
        private System.ComponentModel.IContainer components;
    }
	
}
