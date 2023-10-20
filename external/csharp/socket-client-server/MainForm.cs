/*
 * Created by SharpDevelop.
 * User: sollw2
 * Date: 10/18/2007
 * Time: 7:50 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace NotifyServer
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private TcpListener tcpListen;
		private EventLog eLog;
		private enum balloonIcons
		{
			INFO = ToolTipIcon.Info,
			ERROR = ToolTipIcon.Error,
			WARN = ToolTipIcon.Warning,
			NONE = ToolTipIcon.None
		}
		private enum logIcons
		{
			INFO = EventLogEntryType.Information,
			ERROR = EventLogEntryType.Error,
			WARN = EventLogEntryType.Warning,
			NONE = EventLogEntryType.Information,
		}
	
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			this.Deactivate += new EventHandler(this.sendToTray);
			this.Closed += new EventHandler(this.cleanUp);			
		}
		
		void cleanUp(Object sender, EventArgs e)
		{
			this.notifyIcon.Dispose();
			try
			{
				tcpListen.Stop();
				tcpListen = null;
			}
			catch(System.Exception error)
			{
				
			}
		}
		
		void sendToTray(Object sender, EventArgs e)
		{
			if(this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
			}
		}
		
		void BtnStartClick(object sender, EventArgs e)
		{
			this.listenThread();
		}
		
		void listenThread()
		{
			try
			{
				tcpListen = new TcpListener(new System.Net.IPEndPoint(System.Net.IPAddress.Any, Convert.ToInt16(this.txtPort.Text)));
				tcpListen.Start();
				tcpListen.BeginAcceptTcpClient(new AsyncCallback(this.processEvents), tcpListen);
				this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Now listening on port " + txtPort.Text + ".", ToolTipIcon.Info);
			}
			catch(System.Exception error)
			{
				this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Listener could not be started!", ToolTipIcon.Error);
			}
		}
		
		void processEvents(IAsyncResult asyn)
		{
			try
			{
				StringBuilder myMessage = new StringBuilder("");
				if(!EventLog.SourceExists("NotifyServer"))
				{
					EventLog.CreateEventSource("NotifyServer", "Application");
				}
				eLog = new EventLog();
				eLog.Source = "NotifyServer";
				TcpListener processListen = (TcpListener) asyn.AsyncState;
				TcpClient tcpClient = processListen.EndAcceptTcpClient(asyn);
				NetworkStream myStream = tcpClient.GetStream();
				myMessage.Length = 0;
				string[] messageArray;
				if(myStream.CanRead)
				{
					StreamReader readerStream = new StreamReader(myStream);
					myMessage.Append(readerStream.ReadToEnd());
					messageArray = myMessage.ToString().Split("|".ToCharArray());
					this.notifyIcon.ShowBalloonTip(Convert.ToInt16(messageArray[0]), messageArray[2], messageArray[3], (ToolTipIcon) balloonIcons.Parse(typeof(balloonIcons), messageArray[1]));
					eLog.WriteEntry(messageArray[3], (EventLogEntryType)logIcons.Parse(typeof(logIcons), messageArray[1]));
					readerStream.Close();
				}
				myMessage = null;
				messageArray = null;
				myStream.Close();
				tcpClient.Close();
				eLog.Close();
				tcpListen.BeginAcceptTcpClient(new AsyncCallback(this.processEvents), tcpListen);
			}
			catch(System.Exception error)
			{
				//this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Error reading data from the socket.", ToolTipIcon.Warning);
			}
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			try
			{
				if(tcpListen != null)
				{
					tcpListen.Stop();
					tcpListen = null;
					this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Listener has been stopped.", ToolTipIcon.Info);
				}
				else
				{
					this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Listener is not running.", ToolTipIcon.Info);
				}
			}
			catch(System.Exception error)
			{
				this.notifyIcon.ShowBalloonTip(0, "NotifyServer", "Listener did not stop cleanly.", ToolTipIcon.Warning);
			}
		}
		
		void NotifyIconMouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}
	}
}
