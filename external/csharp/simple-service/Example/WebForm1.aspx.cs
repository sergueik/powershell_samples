using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace WebRequestExample
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.HtmlControls.HtmlInputFile file;
		protected System.Web.UI.WebControls.TextBox TextBox1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button1_Click(object sender, System.EventArgs e)
		{
			//This URL not exist, it's only an example.
			string url = "http://www.asppage/getinfo.asp";
			//Instantiate new CustomWebRequest class
			CustomWebRequest wr = new CustomWebRequest(url);
			//Set values for parameters
			wr.ParamsCollection.Add(new ParamsStruct("user", "george"));
			wr.ParamsCollection.Add(new ParamsStruct("password", "bin"));
			//For file type, send the inputstream of selected file
			wr.ParamsCollection.Add(new ParamsStruct("file", file.PostedFile.InputStream, ParamsStruct.ParamType.File, file.PostedFile.FileName));
			//PostData
			wr.PostData();
			//Set responsestring to textbox1
			TextBox1.Text = wr.ResponseString;
		}
	}
}
