using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;            

namespace TransactionService
{
	/// <summary>
	/// Summary description for ProjectInstaller.
	/// </summary>
	[RunInstaller(true)]
	public class ProjectInstaller : System.Configuration.Install.Installer
	{
		private System.Diagnostics.EventLogInstaller myEventLogInstaller;
		private System.Diagnostics.PerformanceCounterInstaller myPerformanceCounterInstaller;
		private System.ServiceProcess.ServiceProcessInstaller DBWriterServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller DBWriterServiceInstaller;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.myEventLogInstaller = new System.Diagnostics.EventLogInstaller();
			this.myPerformanceCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
			this.DBWriterServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.DBWriterServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// myEventLogInstaller
			// 
			this.myEventLogInstaller.Log = "TransactionLog";
			this.myEventLogInstaller.Source = "TransactionService";
			// 
			// myPerformanceCounterInstaller
			// 
			// this.myPerformanceCounterInstaller.CategoryName = "MyCounters";
			/* this.myPerformanceCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] {
																												  new System.Diagnostics.CounterCreationData("Ctrl", "", System.Diagnostics.PerformanceCounterType.NumberOfItems32)}); */
			// 
			// DBWriterServiceProcessInstaller
			// 
			this.DBWriterServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.DBWriterServiceProcessInstaller.Password = null;
			this.DBWriterServiceProcessInstaller.Username = null;
			// 
			// DBWriterServiceInstaller
			// 
			this.DBWriterServiceInstaller.ServiceName = "DBWriter";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.myEventLogInstaller,
																					  /* this.myPerformanceCounterInstaller, */
																					  this.DBWriterServiceProcessInstaller,
																					  this.DBWriterServiceInstaller});

		}
		#endregion
	}
}
