using HKS.FolderMetadata.Configuration.Internationalization;
using HKS.FolderMetadata.Dialogs.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs
{
	public partial class FrmManage : Form
	{
		public FrmManage()
		{
			InitializeComponent();
			SetIcon();
			TranslateForm();
		}

		public FrmManage(string title)
		: this()
		{
			this.Text = title;
		}

		private void SetIcon()
		{
			ResGetter resGetter = ResGetter.GetShell32Getter();
			if (resGetter != null)
			{
				resGetter.SetFormIcon(this, 269);
			}
		}
		private void TranslateForm()
		{
			btnCancel.Text = Strings.FrmManage_btnCancel_Text;
			btnSave.Text = Strings.FrmManage_btnSave_Text;

			ButtonAdjustor.AdjustButtonsSize(btnCancel, btnSave);

			btnAdd.Text = Strings.FrmManage_btnAdd_Text;
			btnDown.Text = Strings.FrmManage_btnDown_Text;
			btnRemove.Text = Strings.FrmManage_btnRemove_Text;
			btnUp.Text = Strings.FrmManage_btnUp_Text;

			chkSort.Text = Strings.FrmManage_chkSort_Text;
		}

		public string[] GetEntries()
		{
				if (lstEntries.Items.Count == 0)
				{
					return null;
				}

				int length = lstEntries.Items.Count;
				string[] retVal = new string[length];
				for (int i = 0; i < length; i++)
				{
					retVal[i] = (string)lstEntries.Items[i];
				}

				return retVal;
		}

		public void SetEntries(System.Collections.Specialized.StringCollection entries)
		{
			lstEntries.Items.Clear();
			
			if (entries == null)
			{
				return;
			}
			foreach (string entry in entries)
			{
				lstEntries.Items.Add(entry);
			}
		}

		public bool SortEntries
		{
			get
			{
				return chkSort.Checked;
			}
			set
			{
				chkSort.Checked = value;
			}
		}

		private void lstEntries_SelectedIndexChanged(object sender, EventArgs e)
		{
			lstEntries.SelectionChangeManageButtonStates(btnUp, btnDown, btnRemove);
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			lstEntries.MoveUpSelected();
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			lstEntries.MoveDownSelected();
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			lstEntries.RemoveSelected();
		}

		private void chkSort_CheckedChanged(object sender, EventArgs e)
		{
			lstEntries.Enabled = !chkSort.Checked;
		}

		private void txtAdd_TextChanged(object sender, EventArgs e)
		{
			btnAdd.Enabled = (txtAdd.TextLength > 0);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			AddItem();
		}

		private void AddItem()
		{
			if (HasTextInList(txtAdd.Text))
			{
				MessageBox.Show(string.Format(Strings.FrmManage_EntryAlreadyExists, txtAdd.Text), Strings.FrmManage_DuplicateText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			lstEntries.Items.Add(txtAdd.Text);
			txtAdd.Text = "";
		}

		private bool HasTextInList(string text)
		{
			foreach (object entry in lstEntries.Items)
			{
				if (((string)entry) == text)
				{
					return true;
				}
			}

			return false;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void txtAdd_KeyUp(object sender, KeyEventArgs e)
		{
			if (btnAdd.Enabled && e.KeyCode == Keys.Enter)
			{
				AddItem();
			}
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
