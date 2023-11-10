using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace WindowsFormsApplication2 {
	public class NpgsqlRtns {
		public event TickHandler Tick;
		public EventArgs e = null;
		public delegate void TickHandler(NpgsqlRtns c, EventArgs e);
		public string strResult;

		public string Result {
			get {
				return strResult;
			}
		}
		public int StartConv(bool bCreateDB, string strDBName, string strServer, string strUser, string strPassword) {
			int ians = 0;

			ians = doConv(bCreateDB, strDBName, strServer, strUser, strPassword);
			return (ians);
		}

		private int doConv(bool bCreateDB, string strDBName, string strServer, string strUser, string strPassword) {
			int ians = 0;
			string strlastCommand;
			string strCommand;
			FileRtns frtn;
			NpgsqlConnection conn;
			//
			// first use the SA account to create a datbase if requested.  
			//
			//conn = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=Password1;Database=postgres;");
			strCommand = string.Format("Server={0};Port=5432;User Id={1};Password={2};Database=postgres;", strServer, strUser, strPassword);
			conn = new NpgsqlConnection(strCommand);
			conn.Open();
			// if we were requested toc reate the datbase then do so 
			if (bCreateDB) {
				var encoding = "UTF8";
				// "WIN1252" will not work with docker hosted DB
				strCommand = string.Format("CREATE DATABASE \"{0}\" WITH OWNER = postgres ENCODING = '{1}';", strDBName, encoding);
				strlastCommand = strCommand;
				NpgsqlCommand command = new NpgsqlCommand(strCommand, conn);
				try {
					command.ExecuteScalar();
					strResult = "Create Database - Successful";
				} catch {
					strResult = "Error ";
				}
				if (Tick != null) {
					Tick(this, e);
				}
			}
			conn.Close();

			//
			// Now log into the requested database and issue the sql statements. 
			//

			// create a file routines class to read the sql statements form the file. 
			frtn = new FileRtns();
			frtn.FileOpen();
			strCommand = string.Format("Server={1};Port=5432;User Id={2};Password={3};Database={0};", strDBName, strServer, strUser, strPassword);
			conn = new NpgsqlConnection(strCommand);
			conn.Open();
			// while there are statements in the file read them. 
			while (true) {
				strCommand = frtn.FileRead();
				if (strCommand.Length == 0) {
					break;  // we are at the end of the file. 
				}
				strlastCommand = strCommand;
				NpgsqlCommand command = new NpgsqlCommand(strCommand, conn);
				// execute the sql statement. 
				try {
					command.ExecuteScalar();
					strResult = findCommand(strCommand) + " - Successful";
				} catch {
					strResult = findCommand(strCommand) + " - Error";
				}
				if (Tick != null) {
					Tick(this, e);
				}
			}
			conn.Close();
			frtn.FileClose();

			strResult = "Finished";
			if (Tick != null) {
				Tick(this, e);
			}

			return (ians);
		}

		/// <summary>
		/// Find the frst 3 statemetns in the oassed string for display. 
		/// </summary>
		/// <param name="strInp">the input string that has the sql statement</param>
		/// <returns></returns>
		string findCommand(string strInp) {
			int cnt1 = 0;
			int cnt2 = 0;
			string[] stro;
			char[] strses = { ' ', '(' };
			string stroout = "";

			stro = strInp.Split(strses);
			while (cnt1 < stro.Length) {
				if (stro[cnt1].Length != 0) {
					stroout = stroout + stro[cnt1] + " ";
					cnt2++;
					if (cnt2 >= 3)
						break;   // do we have 3 statements
				}
				cnt1++;
			}
			stroout = stroout.Trim();   // trimn the output string incase there are leading/trailing white spaces. 
			return (stroout);
		}
	}
}
