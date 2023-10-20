using HKS.FolderMetadata.Configuration.Internationalization;
using HKS.FolderMetadata.Dialogs.Controls;
using HKS.FolderMetadata.Dialogs.Helpers;
using HKS.FolderMetadata.Generic;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs
{
	/// <summary>
	/// The FrmMain form contains all elements for editing and selection of (existing) metadata.
	/// It also allows to open the <see cref="FrmManage"/> form for editing authors and tags.
	/// </summary>
	public partial class FrmMain : Form
	{
		private readonly Metadata METADATA = null;
		public enum ConfigureMode { None, Authors, Tags };

		/// <summary>
		/// This constructor is called by all other constructors.
		/// </summary>
		public FrmMain()
		{
			InitializeComponent();

			SetText(null); //This will be overridden from any other constructor.
			TranslateForm();
			SetIcon();

			InitializeListSort(lstAuthors, Properties.Settings.Default.SortAuthors);
			InitializeListSort(lstTags, Properties.Settings.Default.SortTags);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="folderPath">The full path to the folder to edit.</param>
		public FrmMain(string folderPath)
		: this()
		{
			SetText(folderPath);

			this.METADATA = new Metadata(folderPath);
			this.METADATA.Load();
			
			this.txtTitle.Text = NoNull(this.METADATA.Title);
			this.txtSubject.Text = NoNull(this.METADATA.Subject);
			this.txtComment.Text = NoNull(this.METADATA.Comment);
			
			lstAuthors.InitializeValues(Properties.Settings.Default.Authors, this.METADATA.Authors);
			SetListToolTip(lstAuthors, this.METADATA.Authors);

			lstTags.InitializeValues(Properties.Settings.Default.Tags, this.METADATA.Tags);
			SetListToolTip(lstTags, this.METADATA.Tags);
		}

		/// <summary>
		/// This constructor is used only when calling the application with the /configure command line parameter. It opens the dialog hidden, opens the "Manage Authors" or "Manage Tags" dialog and then closes the dialog right aways.
		/// </summary>
		/// <param name="configureMode"></param>
		public FrmMain(ConfigureMode configureMode)
		: this(null)
		{
			switch (configureMode)
			{
				case ConfigureMode.Authors:
					ManageAuthors(false);
					break;
				case ConfigureMode.Tags:
					ManageTags(false);
					break;
				default:
					throw new ArgumentOutOfRangeException("configureMode");
			}

			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void InitializeListSort(ListBoxEx listBox, bool sort)
		{
			if (sort)
			{
				listBox.Sorted = true;
			}
		}

		private void SetIcon()
		{
			ResGetter resGetter = ResGetter.GetShell32Getter();
			if (resGetter != null)
			{
				resGetter.SetFormIcon(this, 19);
			}
		}

		private void SetText(string folderPath)
		{
			string format = Strings.FrmMain_Text;
			if (string.IsNullOrEmpty(folderPath))
			{
				folderPath = "";
			}
			else
			{
				folderPath = " - " + folderPath;
			}

			this.Text = string.Format(format, folderPath);
		}

		private void TranslateForm()
		{
			btnManageAuthors.Text = Strings.FrmMain_Manage;
			btnManageTags.Text = Strings.FrmMain_Manage;

			btnCancel.Text = Strings.FrmManage_btnCancel_Text;
			btnSave.Text = Strings.FrmMain_btnSave_Text;
			ButtonAdjustor.AdjustButtonsSize(btnCancel, btnSave);

			lblAuthor.Text = Strings.FrmMain_lblAuthor_Text;
			lblComment.Text = Strings.FrmMain_lblComment_Text;
			lblSubject.Text = Strings.FrmMain_lblSubject_Text;
			lblTags.Text = Strings.FrmMain_lblTags_Text;
			lblTitle.Text = Strings.FrmMain_lblTitle_Text;
		}

		private string NoNull(string s)
		{
			if (s == null)
			{
				return "";
			}

			return s;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Opens the "Manage Authors" dialog and reloads the list values if specified.
		/// </summary>
		/// <param name="reloadValues">If true, reloads the list values.</param>
		private void ManageAuthors(bool reloadValues)
		{
			bool sort = Properties.Settings.Default.SortAuthors;
			System.Collections.Specialized.StringCollection entries = Properties.Settings.Default.Authors;
			if (Manage(ref entries, ref sort, Strings.FrmMain_ManageAuthors))
			{
				Properties.Settings.Default.SortAuthors = sort;
				Properties.Settings.Default.Authors = entries;
				Properties.Settings.Default.Save();
				if (!reloadValues)
				{
					return;
				}

				if (lstAuthors.Sorted != sort)
				{
					lstAuthors.Sorted = sort;
				}

				lstAuthors.ReloadValues(Properties.Settings.Default.Authors, this.METADATA.Authors);
			}
		}

		/// <summary>
		/// Opens the "Manage Tags" dialog and reloads the list values if specified.
		/// </summary>
		/// <param name="reloadValues">If true, reloads the list values.</param>
		private void ManageTags(bool reloadValues)
		{
			bool sort = Properties.Settings.Default.SortTags;
			System.Collections.Specialized.StringCollection entries = Properties.Settings.Default.Tags;
			if (Manage(ref entries, ref sort, Strings.FrmMain_ManageTags))
			{
				Properties.Settings.Default.SortTags = sort;
				Properties.Settings.Default.Tags = entries;
				Properties.Settings.Default.Save();
				if (!reloadValues)
				{
					return;
				}

				if (lstTags.Sorted != sort)
				{
					lstTags.Sorted = sort;
				}
				lstTags.ReloadValues(Properties.Settings.Default.Tags, this.METADATA.Tags);
			}
		}

		/// <summary>
		/// Opens the <see cref="FrmManage"/> dialog in "Manage Authors" or "Manage Tags" mode.
		/// </summary>
		/// <param name="entries">The entries to show</param>
		/// <param name="sort">The setting that specifies if the list is sorted by default.</param>
		/// <param name="title">The window title ("Manage Authors" or "Manage Tags").</param>
		/// <returns></returns>
		private bool Manage(ref System.Collections.Specialized.StringCollection entries, ref bool sort, string title)
		{
			FrmManage frmManage = new FrmManage(title);
			frmManage.SetEntries(entries);
			frmManage.SortEntries = sort;
			if (frmManage.ShowDialog() == DialogResult.OK)
			{
				sort = frmManage.SortEntries;
				if (entries == null)
				{
					entries = new System.Collections.Specialized.StringCollection();
				}
				else
				{
					entries.Clear();
				}
				string[] modEntries = frmManage.GetEntries();
				if (modEntries != null)
				{
					entries.AddRange(modEntries);
				}
				frmManage.Dispose();
				return true;
			}

			frmManage.Dispose();
			return false;
		}

		private void btnManageAuthors_Click(object sender, EventArgs e)
		{
			ManageAuthors(true);
		}

		private void btnManageTags_Click(object sender, EventArgs e)
		{
			ManageTags(true);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (this.METADATA.HasFilePath)
			{
				this.METADATA.Authors = lstAuthors.GetSelectedValues();
				this.METADATA.Comment = txtComment.Text;
				this.METADATA.Subject = txtSubject.Text;
				this.METADATA.Tags = lstTags.GetSelectedValues();
				this.METADATA.Title = txtTitle.Text;
				this.METADATA.Save();
			}
			this.Close();
		}

		private void lst_Click(object sender, EventArgs e)
		{
			ListBoxEx listBox = (ListBoxEx)sender;
			List<string> values = listBox.GetSelectedValues();
			SetListToolTip(listBox, values);
		}

		private void SetListToolTip(ListBox listBox, List<string> values)
		{
			string text = ListHelper.ToString(values);
			toolTip1.SetToolTip(listBox, text);
		}

		protected override void WndProc(ref Message m)
		{
			if (LanguageConfigurator.ForceShowMnemonics)
			{
				WndProcOverrides.ShowMnemonics(ref m);
			}
			base.WndProc(ref m);
		}
	}
}
