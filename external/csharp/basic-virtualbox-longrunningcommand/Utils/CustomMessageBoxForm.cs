using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

// restored
// origin: http://www.codeproject.com/Tips/827370/Custom-Message-Box-DLL
namespace Utils {

    public class CustomMessageBoxForm {
        private Form f;

        private Button buttonDetails;
        private Button buttonOK;
        private Button buttonYes;
        private Button buttonNo;
        private Button buttonCancel;
        private Button buttonAbort;
        private Button buttonRetry;
        private Button buttonIgnore;

        private TextBox txtDescription;
        private PictureBox iconBitmap;
        private Panel panel;
        private Label labelMessage;

        private string result;

        public static string Show( string messageText) {
            return Show( messageText, null, null, "Information", "OK");
        }

        public static string Show( string messageText, Exception e) {
            return Show( messageText, "Exception", e.Message, e.StackTrace, "OK");
        }
 
        public static string Show( string messageText, string messageTitle, string description) {
            return Show( messageText, messageTitle, description, "Information", "OK");
        }

        public static string Show( string messageText, string messageTitle, string description, string icon){ 
            return Show( messageText, messageTitle, description,  icon, "OK");
        }

        public static string Show( string messageText, string messageTitle, string description, string icon, string buttons, string filename ) {

            var box = new CustomMessageBoxForm();

            box.Initialize();

            box.SetMessageText(  messageText, messageTitle, description);
            box.AddIconBitmap(filename, null );
            box.AddButtons(buttons);
            box.result = "Cancel";
            box.DrawBox();
            box.f.ShowDialog();
            string answer = box.result;
            box.f.Dispose();
            return answer;
        }

        public static string Show( string messageText, string messageTitle, string description, string icon, string buttons ) {
            var box = new CustomMessageBoxForm();

            box.Initialize();

            box.SetMessageText(  messageText, messageTitle, description);
            box.AddIconBitmap(icon);
            box.AddButtons(buttons);
            box.result = "Cancel";
            box.DrawBox();
            box.f.ShowDialog();
            string answer = box.result;
            box.f.Dispose();
            return answer;
        }


        private void Initialize() {
            f = new Form();

            buttonDetails = new Button();
            buttonOK = new Button();
            buttonYes = new Button();
            buttonNo = new Button();
            buttonCancel = new Button();
            buttonAbort = new Button();
            buttonRetry = new Button();
            buttonIgnore = new Button();

            txtDescription = new TextBox();
            iconBitmap = new PictureBox();
            panel = new Panel();
            labelMessage = new Label();
        }


        private void ReturnResponse(  object sender, EventArgs e) {

            var button = sender as Button;
            if (button == null) {
                return;
            }

            switch (button.Text)  {
                case "Yes":
                case "No":
                case "OK":
                case "Cancel":
                case "Abort":
                case "Retry":
                case "Ignore":

                    result = button.Text;
                    break;
            }

            f.Dispose();
        }

		// Powershell
        /*
		function add_icon_bitmap {
			  param([psobject]$param)

			  switch ($param)
			  {
			    ('Error') {
			      $icon_bitmap.Image = ([System.Drawing.SystemIcons]::Error).ToBitmap()
			    }
			    ('Information') {
			      $icon_bitmap.Image = ([System.Drawing.SystemIcons]::Information).ToBitmap()
			    }
			    ('Question') {
			      $icon_bitmap.Image = ([System.Drawing.SystemIcons]::Question).ToBitmap()
			    }
			    ('Warning') {
			      $icon_bitmap.Image = ([System.Drawing.SystemIcons]::Warning).ToBitmap()
			    }
			    default {
			      $icon_bitmap.Image = ([System.Drawing.SystemIcons]::Information).ToBitmap()
			    }
			  }
			}
        */

        private void AddIconBitmap( string icon) {
            switch (icon) {
                case "Error":
                    iconBitmap.Image =
                        SystemIcons.Error.ToBitmap();
                    break;

                case "Information":
                    iconBitmap.Image =
                        SystemIcons.Information.ToBitmap();
                    break;

                case "Question":
                    iconBitmap.Image =
                        SystemIcons.Question.ToBitmap();
                    break;

                case "Warning":
                    iconBitmap.Image =
                        SystemIcons.Warning.ToBitmap();
                    break;

                default:
                    iconBitmap.Image =
                        SystemIcons.Information.ToBitmap();
                    break;
            }
        }
        private void AddIconBitmap( string filename, string icon = null ) {

			string iconPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, filename);
			
			if (File.Exists(iconPath)) {
				iconBitmap.Image = new Icon(iconPath).ToBitmap();
			}
        }
        private void SetMessageText( string messageText, string title,  string description) {
            labelMessage.Text = messageText;

            if (!String.IsNullOrEmpty(description)) {
                txtDescription.Text = description;
            } else {
                buttonDetails.Visible = false;
            }

            if (!String.IsNullOrEmpty(title)) {
                f.Text = title;
            } else {
                f.Text = "Your Message Box";
            }
        }


        private void ClickHandler(   object sender, EventArgs e) {
            if (buttonDetails.Tag.ToString() == "collapse") {
                f.Height += txtDescription.Height + 6;

                buttonDetails.Tag = "expand";
                buttonDetails.Text = "Hide Details";

                txtDescription.WordWrap = true;
            } else if (buttonDetails.Tag.ToString() == "expand") {
                f.Height -= txtDescription.Height + 6;

                buttonDetails.Tag = "collapse";
                buttonDetails.Text = "Show Details";
            }
        }

		private void AddButtons(string buttons) {
		    var start = new Point(361, 84);

		    switch (buttons) {
		        case "None":
		        case "OK":
		            buttonOK = CreateButton( "OK", start);
		            break;

		        case "YesNo":
		            buttonNo = CreateButton( "No", start);
		            buttonYes = CreateButton(  "Yes", new Point(  buttonNo.Location.X - buttonNo.Width - 8,  buttonNo.Location.Y));
		            break;

		        case "YesNoCancel":
		            buttonCancel = CreateButton(  "Cancel", start);
		            buttonNo = CreateButton( "No", new Point( buttonCancel.Location.X - buttonCancel.Width - 8, buttonCancel.Location.Y));
		            buttonYes = CreateButton( "Yes", new Point( buttonNo.Location.X - buttonNo.Width - 8, buttonNo.Location.Y));
		            break;

		        case "RetryCancel":
		            buttonCancel = CreateButton( "Cancel", start);
		            buttonRetry = CreateButton( "Retry", new Point(buttonCancel.Location.X - buttonCancel.Width - 8, buttonCancel.Location.Y));
		            break;

		        case "AbortRetryIgnore":

		            buttonIgnore = CreateButton( "Ignore", start);
		            buttonRetry = CreateButton( "Retry", new Point( buttonIgnore.Location.X - buttonIgnore.Width - 8, buttonIgnore.Location.Y));
		            buttonAbort = CreateButton( "Abort", new Point( buttonRetry.Location.X - buttonRetry.Width - 8,  buttonRetry.Location.Y));
		            break;
		    }
		}

		private void DrawBox() {
		    f.Controls.Add(panel);
		
		    panel.Dock = DockStyle.Fill;
		
		
		    // icon
		
		    iconBitmap.Height = 36;
		    iconBitmap.Width = 40;
		    iconBitmap.Location = new Point(10, 11);
		
		    panel.Controls.Add(iconBitmap);
		
		
		    // details text box
		
		    txtDescription.Multiline = true;
		    txtDescription.Height = 183;
		    txtDescription.Width = 464;
		
		    txtDescription.Location =
		        new Point(6, 143);
		
		    txtDescription.BorderStyle =
		        BorderStyle.Fixed3D;
		
		    txtDescription.ScrollBars =
		        ScrollBars.Both;
		
		    txtDescription.ReadOnly = true;
		
		    panel.Controls.Add(txtDescription);
		
		
		
		    // details button
		
		    buttonDetails.Height = 24;
		    buttonDetails.Width = 96;
		
		    buttonDetails.Location =
		        new Point(6, 84);
		
		    buttonDetails.Tag = "expand";
		    buttonDetails.Text = "Show Details";
		
		    buttonDetails.Click += ClickHandler;
		
		    panel.Controls.Add(buttonDetails);
		
		
		
		    // message label
		
		    labelMessage.Location =
		        new Point(64, 22);
		
		    labelMessage.AutoSize = true;
		
		    panel.Controls.Add(labelMessage);
		
		
		
		    // form setup
		
		    f.Height = 360;
		    f.Width = 483;
		
		    f.StartPosition =
		        FormStartPosition.CenterScreen;
		
		    f.FormBorderStyle =
		        FormBorderStyle.FixedSingle;
		
		    f.MaximizeBox = false;
		    f.MinimizeBox = false;
		
		    f.BackColor =
		        SystemColors.ButtonFace;
		
		
		
		    // collapse details initially
		
		    if (buttonDetails.Tag.ToString() == "expand")
		    {
		        f.Height =
		            f.Height - txtDescription.Height - 6;
		
		        buttonDetails.Tag = "collapse";
		        buttonDetails.Text = "Show Details";
		    }
		}
		private Button CreateButton(string text, Point location) {
		    var button = new Button();

		    button.Width = 80;
		    button.Height = 24;
		    button.Location = location;
		    button.Text = text;
		    button.Click += ReturnResponse;
		    panel.Controls.Add(button);

		    return button;
		}
	}
}
