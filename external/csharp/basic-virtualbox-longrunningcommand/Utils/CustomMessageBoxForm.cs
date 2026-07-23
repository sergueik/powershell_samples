using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

// restored
// origin: http://www.codeproject.com/Tips/827370/Custom-Message-Box-DLL
namespace Utils {

    public class CustomMessageBoxForm {
        private Form form;

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
        private IntPtr handle;
			public IntPtr getHandle(){ return this.form.Handle;
		}
        public static string Show( string messageText) {
            return Show( messageText, null, null, SystemIcons.Information);
        }

        public static string Show( string messageText, Exception e) {
            return Show( e.Message,  "Exception", "Stack Trace: \n" + e.StackTrace , SystemIcons.Error);
        }

        public static string Show( string messageText, string messageTitle, string description) {
            return Show( messageText, messageTitle, description, SystemIcons.Information, "OK");
        }

        public static string Show( string messageText, string messageTitle, string description, Icon icon){
            return Show( messageText, messageTitle, description,  icon, "OK");
        }

        public static string Show( string messageText, string messageTitle, string description, string filename , string buttons) {

            var box = new CustomMessageBoxForm();
            box.Initialize();
            box.SetMessageText( messageText, messageTitle, description);
            box.AddIconBitmap(filename);
            box.AddButtons(buttons);
            box.result = "Cancel";
            box.DrawBox();
            box.form.ShowDialog();
            string answer = box.result;
            box.form.Dispose();
            return answer;
        }

        public static string Show( string messageText, string messageTitle, string description, Icon icon, string buttons ) {
            var box = new CustomMessageBoxForm();

            box.Initialize();

            box.SetMessageText( messageText, messageTitle, description);
            box.AddIconBitmap(icon);
            box.AddButtons(buttons);
            box.result = "Cancel";
            box.DrawBox();
            box.form.ShowDialog();
            string answer = box.result;
            box.form.Dispose();
            return answer;
        }

        private void Initialize() {
            form = new Form();

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

        private void ReturnResponse( object sender, EventArgs e) {

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

            form.Dispose();
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.drawing.systemicons?view=netframework-4.5
        private void AddIconBitmap( Icon icon = null ) {
			iconBitmap.Image = (icon ?? SystemIcons.Information).ToBitmap();
        }

        // https://learn.microsoft.com/en-us/dotnet/api/system.drawing.icon?view=netframework-4.5
        private void AddIconBitmap( string filename ) {
        	if (filename == null)
        		iconBitmap.Image = SystemIcons.Information.ToBitmap();
    		else {
				string iconPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, filename);
				if (File.Exists(iconPath)) {
					using (Image image = Image.FromFile(iconPath)) {
            			iconBitmap.Image = new Bitmap(image);
        			}
					// iconBitmap.Image = new Icon(iconPath).ToBitmap();
				}
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
                form.Text = title;
            } else {
                form.Text = "Your Message Box";
            }
        }


        private void ClickHandler(   object sender, EventArgs e) {
            if (buttonDetails.Tag.ToString() == "collapse") {
                form.Height += txtDescription.Height + 6;

                buttonDetails.Tag = "expand";
                buttonDetails.Text = "Hide Details";

                txtDescription.WordWrap = true;
            } else if (buttonDetails.Tag.ToString() == "expand") {
                form.Height -= txtDescription.Height + 6;

                buttonDetails.Tag = "collapse";
                buttonDetails.Text = "Show Details";
            }
        }

		private void AddButtons(string buttons) {
		    var start = new Point(361, 84);

		    switch (buttons) {
		        case "None":
		            break;
		        case "Really?":
		            buttonOK = CreateButton( "Really?", start);
		            break;

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
		    form.Controls.Add(panel);

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

		    txtDescription.Location = new Point(6, 143);

		    txtDescription.BorderStyle = BorderStyle.Fixed3D;

		    txtDescription.ScrollBars = ScrollBars.Both;

		    txtDescription.ReadOnly = true;

		    panel.Controls.Add(txtDescription);

		    // details button

		    buttonDetails.Height = 24;
		    buttonDetails.Width = 96;

		    buttonDetails.Location = new Point(6, 84);

		    buttonDetails.Tag = "expand";
		    buttonDetails.Text = "Show Details";

		    buttonDetails.Click += ClickHandler;

		    panel.Controls.Add(buttonDetails);

		    // message label

		    labelMessage.Location =  new Point(64, 22);

		    labelMessage.AutoSize = true;

		    panel.Controls.Add(labelMessage);

		    // form setup

		    form.Height = 360;
		    form.Width = 483;

		    form.StartPosition = FormStartPosition.CenterScreen;

		    form.FormBorderStyle = FormBorderStyle.FixedSingle;

		    form.MaximizeBox = false;
		    form.MinimizeBox = false;

		    form.BackColor = SystemColors.ButtonFace;

		    // collapse details initially

		    if (buttonDetails.Tag.ToString() == "expand") {
		        form.Height = form.Height - txtDescription.Height - 6;
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
