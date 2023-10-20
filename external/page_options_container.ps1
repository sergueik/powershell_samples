

# https://github.com/rlipscombe/paged-options-dialog
Add-Type -TypeDefinition @"
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace PagedOptionsDialog
{

    // Does not suppress 
    // Warning as Error:
    // The variable 'ex' is declared but never used

#pragma warning disable 0168
#pragma warning disable 0169
#pragma warning disable 0649

    public partial class OptionsDialog : Form
    {
        private System.ComponentModel.IContainer components = null;
        private PropertyPage _activePage;
        public IList<PropertyPage> Pages { get; private set; }
        private string imgFolderPath = Directory.GetCurrentDirectory();
        // OptionsDialog.Designer.cs

        private System.Windows.Forms.Panel pagePanel;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ImageList imageList;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pagePanel = new System.Windows.Forms.Panel();
            this.listPanel = new System.Windows.Forms.Panel();
            this.listView = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.listPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pagePanel
            // 
            this.pagePanel.BackColor = System.Drawing.SystemColors.Control;
            this.pagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pagePanel.Location = new System.Drawing.Point(95, 0);
            this.pagePanel.Name = "pagePanel";
            this.pagePanel.Size = new System.Drawing.Size(375, 308);
            this.pagePanel.TabIndex = 0;
            // 
            // listPanel
            // 
            this.listPanel.BackColor = System.Drawing.SystemColors.Control;
            this.listPanel.Controls.Add(this.listView);
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.listPanel.Location = new System.Drawing.Point(0, 0);
            this.listPanel.Name = "listPanel";
            this.listPanel.Padding = new System.Windows.Forms.Padding(3);
            this.listPanel.Size = new System.Drawing.Size(95, 308);
            this.listPanel.TabIndex = 1;
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.LargeImageList = this.imageList;
            this.listView.Location = new System.Drawing.Point(3, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(89, 302);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPanel.Controls.Add(this.cancelButton);
            this.buttonPanel.Controls.Add(this.okButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 308);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(470, 46);
            this.buttonPanel.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(383, 11);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(302, 11);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(470, 354);
            this.Controls.Add(this.pagePanel);
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.buttonPanel);
            this.Name = "OptionsDialog";
            this.Text = "OptionsDialog";
            this.Load += new System.EventHandler(this.OptionsDialog_Load);
            this.listPanel.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        public OptionsDialog()
        {
            Pages = new List<PropertyPage>();

            InitializeComponent();
        }

        private void OptionsDialog_Load(object sender, System.EventArgs e)
        {

            // var defaultImage = new Bitmap(GetType(), "Bitmaps.DefaultOptionsPage.bmp");
            try
            {
                Console.WriteLine(Path.Combine(imgFolderPath, @"DefaultOptionsPage.bmp"));
                // var defaultImage = new Bitmap(Path.Combine(imgFolderPath, @"DefaultOptionsPage.bmp"));
                // imageList.Images.Add(defaultImage);
            }
            catch (Exception) { }

            AddPageControls();
            SelectFirstListItem();
        }

        private void SelectFirstListItem()
        {
            if (listView.Items.Count != 0)
                listView.Items[0].Selected = true;
        }

        private void AddPageControls()
        {
            var maxPageSize = pagePanel.Size;
            foreach (var page in Pages)
            {
                AddPage(page, ref maxPageSize);
            }

            SizeToFit(maxPageSize);
            CenterToParent();
        }

        private void SizeToFit(Size maxPageSize)
        {
            var newSize = new Size();
            newSize.Width = maxPageSize.Width + (Width - pagePanel.Width);
            newSize.Height = maxPageSize.Height + (Height - pagePanel.Height);

            Size = newSize;
        }

        private void AddPage(PropertyPage page, ref Size maxPageSize)
        {
            pagePanel.Controls.Add(page);

            AddListItemForPage(page);

            // Adjust to fit the largest child page.
            if (page.Width > maxPageSize.Width)
                maxPageSize.Width = page.Width;
            if (page.Height > maxPageSize.Height)
                maxPageSize.Height = page.Height;

            // Set page.Dock *after* looking at its size.
            page.Dock = DockStyle.Fill;
            page.Visible = false;
        }

        private void AddListItemForPage(PropertyPage page)
        {
            int imageIndex = 0;

            var image = page.Image;
            if (image != null)
            {
                imageList.Images.Add(image);
                imageIndex = imageList.Images.Count - 1;
            }

            var item = new ListViewItem(page.Title, imageIndex);
            item.Tag = page;

            listView.Items.Add(item);
        }

        private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (_activePage != null)
                _activePage.Visible = false;

            if (listView.SelectedItems.Count != 0)
            {
                var selectedItem = listView.SelectedItems[0];
                var page = (PropertyPage)selectedItem.Tag;
                _activePage = page;
            }

            if (_activePage != null)
            {
                _activePage.Visible = true;
                _activePage.OnSetActive();
            }
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            foreach (var propertyPage in Pages)
            {
                propertyPage.OnApply();
            }
        }


    }

    public class PropertyPage : UserControl
    {
        public virtual string Title
        {
            get { return GetType().Name; }
            set { }
        }

        public virtual Image Image
        {
            get { return null; }
        }

        public virtual void OnSetActive()
        {

        }

        public virtual void OnApply()
        {
            
        }
    }

    // TODO: collapse the inheritance 
    public class AdvancedOptionsPage : PropertyPage
    {
        private System.ComponentModel.IContainer components = null;
        private string imgFolderPath = Directory.GetCurrentDirectory();
        private string _imageFile = @"AdvancedOptionsPage.bmp";

        private string _title = "Options Page";
        public override string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string ImageFile
        {
            get { return _imageFile; }
            set { _imageFile = value; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout(  );
            this.Size = new System.Drawing.Size( 338, 150 );
            this.ResumeLayout( false );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public AdvancedOptionsPage()
        {
            InitializeComponent();
        }

        public override Image Image
        {
            get
            {
                string imageFilePath = null;
                Image _image;
                try
                {
                    imageFilePath = Path.Combine(imgFolderPath, _imageFile);
                    // the path will vary
                    Console.Error.WriteLine(imageFilePath);
                    _image = new Bitmap(imageFilePath);
                }
                catch (Exception)
                {
                    return null;
                }

                return _image ;
            }
        }
    }
}
"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll','System.Xml.dll'


Add-Type -TypeDefinition @" 

// "
using System;
using System.Windows.Forms;
public class Win32Window : IWin32Window
{
    private IntPtr _hWnd;
    private string _data;

    public string Data
    {
        get { return _data; }
        set { _data = value; }
    }

    public Win32Window(IntPtr handle)
    {
        _hWnd = handle;
    }

    public IntPtr Handle
    {
        get { return _hWnd; }
    }

}

"@ -ReferencedAssemblies 'System.Windows.Forms.dll'


@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }



$o = New-Object 'PagedOptionsDialog.OptionsDialog'

$d1 = New-Object 'PagedOptionsDialog.AdvancedOptionsPage'
$o.Pages.Add($d1)


$d2 = New-Object 'PagedOptionsDialog.AdvancedOptionsPage'
$d1.Title = 'More Options Page'  
$o.Pages.Add($d2)

$d3 = New-Object 'PagedOptionsDialog.AdvancedOptionsPage'
$o.Pages.Add($d3)

$d4 = New-Object 'PagedOptionsDialog.AdvancedOptionsPage'
$d4.ImageFile = 'GenericSettingsConfig.bmp'
$d4.Title = 'Configuration'  

$o.Pages.Add($d4)

$o.ShowDialog()

 