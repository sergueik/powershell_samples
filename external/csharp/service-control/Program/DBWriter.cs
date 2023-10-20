using System;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;

using System.Threading;
using System.Timers;
using System.ComponentModel;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

namespace TransactionService
{
	public class DBWriter : System.ServiceProcess.ServiceBase
	{

		private System.ComponentModel.Container components = null;
		private EventLog myLog;
		// private PerformanceCounter myPerformanceCounter;
		
		private int Interval2 = 60000;
		private int Interval1 = 1000;
		System.Timers.Timer Timer1 = new System.Timers.Timer();
		System.Timers.Timer Timer2 = new System.Timers.Timer();

		
		public DBWriter()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new DBWriter() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// DBWriter
			// 
			this.AutoLog = false;
			this.ServiceName = "DBWriter";
			this.myLog = new EventLog();
//			if(! EventLog.Exists("TransactionLog"))
//			{
//				EventLog.CreateEventSource("TransactionService","TransactionLog");
//			}
			this.myLog.Log = "TransactionLog";
			this.myLog.Source = "TransactionService";
			/*
			this.myPerformanceCounter = new PerformanceCounter();
			this.myPerformanceCounter.BeginInit();
			this.myPerformanceCounter.CategoryName = "MyCounters";
			this.myPerformanceCounter.CounterName = "Ctrl";
			*/


		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			//Create Transaction File and Close it
			FileStream fsTramsaction = File.Create ("c:\\Transaction.tmp");
			fsTramsaction.Close();

			//Create Customers DB File if its not Exists
			if (! File.Exists("c:\\customers.db"))
			{
				FileStream fsCustomers = File.Create ("c:\\customers.db");
				fsCustomers.Close();
			}
			
			Timer1.Elapsed += new ElapsedEventHandler(OnElapsedTimer1);
			Timer1.Interval = Interval1;
			Timer1.Enabled = true;
			Timer2.Elapsed += new ElapsedEventHandler(OnElapsedTimer2);
			Timer2.Interval = Interval2;
			Timer2.Enabled = true;

			//Write to Transactoin Log
			myLog.WriteEntry("DBWriter Serice Started Successfully on " + System.DateTime.Now.ToString() , EventLogEntryType.Information );

		}
 
		
				private void OnElapsedTimer1(object source, ElapsedEventArgs args) {
				
				try {
					var performanceCounter = new PerformanceCounter();
					performanceCounter.CategoryName = "System";
					performanceCounter.CounterName = "Processor Queue Length";
					performanceCounter.InstanceName = null;
					var value = (Int32)performanceCounter.RawValue;
					/// var row = new Data();
					// row.TimeStamp = DateTime.Now;
					// row.Value = value;
					// buffer.AddLast(row);
					this.myLog.WriteEntry("procesing Timer1"/* + row.ToString()*/ ) ;
				}  catch (System.InvalidOperationException e) {
   					// Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
   					this.myLog.WriteEntry("procesing Timer1 Exception: "  + e.ToString());
   				}
		}
/*
		private void AverageData() {
			var rows = buffer.ToList();
			var values = (from row in rows select row.Value);
			var result = values.Average();
			this.myLog.WriteEntry("Average: " + result);
		}
		*/
		private void OnElapsedTimer2(object source, ElapsedEventArgs args) {
			int result = 0;
			this.myLog.WriteEntry("Average: " + result);
		}

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			// Delete the Transaction.tmp File
			File.Delete("c:\\Transaction.tmp");

			//Set the Performance Counter value to 0
			// this.myPerformanceCounter.RawValue = 0;

			//Write Entry to Event Log
			this.myLog.WriteEntry("Service Stopped Successfully on " + DateTime.Now , EventLogEntryType.Information);
		}

		protected override void OnCustomCommand(int command)
		{
			base.OnCustomCommand (command);
			switch (command)
			{
				case(201) : Commit();break;
				case (200) : RollBack();break;

			}
		}

		private void Commit()
		{
			//Create a StreamReader to read data from the Transaction.tmp file
			StreamReader srTransaction = new StreamReader(new FileStream("c:\\Transaction.tmp",FileMode.Open ));
			
			//Create StreamWriter to write data to Customer.Db file
			StreamWriter swCustomers = new StreamWriter("c:\\Customers.db");
			swCustomers.Write("test");
			swCustomers.Write(srTransaction.ReadToEnd());
			swCustomers.Flush();

			//Close the Streams
			srTransaction.Close();
			swCustomers.Close();

			//Increment the performance Counter
			// this.myPerformanceCounter.Increment();

			this.myLog.WriteEntry("DBWriter Service Committed Transaction on " + DateTime.Now.ToString(),EventLogEntryType.Information );

			TruncateTransactionFile();
		
		}

		private void RollBack()
		{
			TruncateTransactionFile();
			this.myLog.WriteEntry("DBWriter RollBacked Tranaction on " + DateTime.Now );
		}

		private void TruncateTransactionFile()
		{
			//Delete the Data from Transaction.tmp file
			FileStream fsTransaction = new FileStream("c:\\Transaction.tmp",FileMode.Truncate);
			fsTransaction.Flush();
			fsTransaction.Close();

		}

	}
}
