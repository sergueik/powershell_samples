using HKS.FolderMetadata.Extensions;
using HKS.FolderMetadata.Generic;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Dialogs.Controls
{
	/// <summary>
	/// ListBoxEx inherits from <see cref="System.Windows.Forms.ListBox"/>. It adds functions for managing list box values.
	/// </summary>
	public class ListBoxEx : ListBox
	{
		/// <summary>
		/// Initializes the values from settings and metadata.
		/// </summary>
		/// <param name="valuesFromSettings"></param>
		/// <param name="valuesFromMetadata"></param>
		public void InitializeValues(System.Collections.Specialized.StringCollection valuesFromSettings, List<string> valuesFromMetadata)
		{
			LoadValues(valuesFromSettings, valuesFromMetadata, true);
		}

		/// <summary>
		/// Reloads the values and selects the previously selected items.
		/// </summary>
		/// <param name="valuesFromSettings"></param>
		/// <param name="valuesFromMetadata"></param>
		public void ReloadValues(System.Collections.Specialized.StringCollection valuesFromSettings, List<string> valuesFromMetadata)
		{
			List<string> selectedItems = GetSelectedValues(); //Backup selection
			LoadValues(valuesFromSettings, valuesFromMetadata, false);
			SetSelectedItems(selectedItems); //Restore selection
		}

		/// <summary>
		/// Loads the values from settings and metadata
		/// </summary>
		/// <param name="valuesFromSettings"></param>
		/// <param name="valuesFromMetadata"></param>
		/// <param name="selectMetadataItems"></param>
		private void LoadValues(System.Collections.Specialized.StringCollection valuesFromSettings, List<string> valuesFromMetadata, bool selectMetadataItems)
		{
			base.Items.Clear();
			if (valuesFromSettings != null)
			{
				FillValues(valuesFromSettings.ToListOfString());
			}
			FillValues(valuesFromMetadata, selectMetadataItems);
		}

		/// <summary>
		/// Returns all selected items from the list box.
		/// </summary>
		/// <returns></returns>
		public List<string> GetSelectedValues()
		{
			List<string> retVal = new List<string>();
			foreach (object selectedObject in base.SelectedItems)
			{
				string selectedItem = selectedObject as string;
				if (!string.IsNullOrEmpty(selectedItem))
				{
					retVal.Add(selectedItem);
				}
			}

			return retVal;
		}

		public List<int> GetSelectedIndexes()
		{
			List<int> retVal = new List<int>();
			int length = base.Items.Count;
			for (int i = 0; i < length; i++)
			{
				if (base.GetSelected(i))
				{
					retVal.Add(i);
				}
			}

			return retVal;
		}

		/// <summary>
		/// Selects a single item in the list box.
		/// </summary>
		/// <param name="item">The item to select</param>
		/// <param name="value">If true, selects the item, otherwise de-selects the item.</param>
		public void SetSelectedItem(string item, bool value = true)
		{
			if (string.IsNullOrEmpty(item))
			{
				return;
			}

			int index = base.Items.IndexOf(item);
			if (index > -1)
			{
				base.SetSelected(index, value);
			}
		}

		/// <summary>
		/// Selects several items in the list box.
		/// </summary>
		/// <param name="items">The items to select.</param>
		/// <param name="value">If true, selects the items, otherwise de-selects the items.</param>
		public void SetSelectedItems(List<string> items, bool value = true)
		{
			if (items == null)
			{
				return;
			}

			foreach (string item in items)
			{
				SetSelectedItem(item, value);
			}
		}

		/// <summary>
		/// Selects the items with the specified indexes.
		/// </summary>
		/// <param name="indexes">The indexes for selection.</param>
		/// <param name="value">If true, selects the items, otherwise de-selects the items.</param>
		public void SetSelected(List<int> indexes, bool value = true)
		{
			if (indexes == null)
			{
				return;
			}
			foreach (int index in indexes)
			{
				base.SetSelected(index, value);
			}
		}

		/// <summary>
		/// Adds the specified items to the listbox if they do not exist yet.
		/// </summary>
		/// <param name="values">The values to add.</param>
		private void FillValues(List<string> values)
		{
			if (values == null)
			{
				return;
			}

			foreach (string value in values)
			{
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}

				if (!base.Items.Contains(value)) //Check if we added the item before.
				{
					base.Items.Add(value);//Item does not exist, so we add it
				}
			}
		}

		/// <summary>
		/// Adds the specified items to the listbox if they do not exist yet.
		/// </summary>
		/// <param name="values">The items to add.</param>
		/// <param name="selectItem">If true, selects the items.</param>
		private void FillValues(List<string> values, bool selectItem)
		{
			if (values == null)
			{
				return;
			}

			foreach (string value in values)
			{
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}

				int index = base.Items.IndexOf(value); //Check if we added the item before.
				if (index < 0)
				{
					index = base.Items.Add(value);//Item does not exist, so we add it
				}

				if (selectItem)
				{
					base.SetSelected(index, true); //Select the item.
				}
			}
		}


		/// <summary>
		/// Moves down the specified list item.
		/// </summary>
		/// <param name="index">The index of the item to move down.</param>
		/// <returns>Returns the new item index.</returns>
		public int MoveDown(int index)
		{
			object entry = base.Items[index];
			base.Items.RemoveAt(index);

			index++;

			base.Items.Insert(index, entry);
			return index;
		}

		/// <summary>
		/// Moves down all specified list items.
		/// </summary>
		/// <param name="indexes">The indexes of the items to move down.</param>
		/// <returns>Returns new indexes.</returns>
		public List<int> MoveDown(List<int> indexes)
		{
			List<int> retVal = new List<int>();
			int lastItemIndex = base.Items.Count - 1;
			if (lastItemIndex < 0)
			{
				return retVal;
			}

			List<int> idxs = new List<int>(indexes);
			idxs.Sort();

			bool lastItemSelected = false;
			for (int i = idxs.Count - 1; i > -1; i--)
			{
				int index = idxs[i];
				if (index == lastItemIndex)
				{
					retVal.Add(index);
					lastItemSelected = true;
					continue;
				}

				index = MoveDown(index);

				if (index == lastItemIndex && lastItemSelected)
				{
					retVal.Add(index + 1);
					lastItemSelected = false;
					continue;
				}
				
				retVal.Add(index);
			}

			return retVal;
		}

		/// <summary>
		/// Moves down all selected list items.
		/// </summary>
		public void MoveDownSelected()
		{
			List<int> selectedIndexes = GetSelectedIndexes();
			selectedIndexes = MoveDown(selectedIndexes);
			SetSelected(selectedIndexes, true);
		}

		/// <summary>
		/// Moves up the specified list item.
		/// </summary>
		/// <param name="index">The index of the item to move up.</param>
		/// <returns>Returns the new item index.</returns>
		public int MoveUp(int index)
		{
			object entry = base.Items[index];
			base.Items.RemoveAt(index);

			index--;

			base.Items.Insert(index, entry);
			return index;
		}

		/// <summary>
		/// Moves up all specified list items.
		/// </summary>
		/// <param name="indexes">The indexes of the items to move up.</param>
		/// <returns>Returns new indexes.</returns>
		public List<int> MoveUp(List<int> indexes)
		{
			List<int> retVal = new List<int>();
			if (base.Items.Count == 0)
			{
				return retVal;
			}

			List<int> idxs = new List<int>(indexes);
			idxs.Sort();
			int length = idxs.Count;

			bool firstItemSelected = false;
			for (int i = 0; i < length; i++)
			{
				int index = idxs[i];
				if (index == 0)
				{
					retVal.Add(0);
					firstItemSelected = true;
					continue;
				}

				index = MoveUp(index);

				if (index == 0 && firstItemSelected)
				{
					retVal.Add(1);
					firstItemSelected = false;
					continue;
				}

				retVal.Add(index);
			}

			return retVal;
		}

		/// <summary>
		/// Moves up all selected list items.
		/// </summary>
		public void MoveUpSelected()
		{
			List<int> selectedIndexes = GetSelectedIndexes();
			selectedIndexes = MoveUp(selectedIndexes);
			SetSelected(selectedIndexes, true);
		}

		/// <summary>
		/// Removes all specified list items.
		/// </summary>
		/// <param name="indexes">The indexes of the items to remove.</param>
		public void RemoveItems(List<int> indexes)
		{
			if (indexes == null || base.Items.Count == 0)
			{
				return;
			}

			List<int> idxs = new List<int>(indexes);
			idxs.Sort();

			for (int i = idxs.Count - 1; i > -1; i--)
			{
				int index = idxs[i];
				base.Items.RemoveAt(index);
			}
		}
		
		/// <summary>
		/// Removes all selected list items.
		/// </summary>
		/// <returns>Removes the number of removed items.</returns>
		public int RemoveSelected()
		{
			List<int> selectedIndexes = GetSelectedIndexes();
			RemoveItems(selectedIndexes);
			return selectedIndexes.Count;
		}

		/// <summary>
		/// Enables or disables the specified buttons depending on the current selection.
		/// </summary>
		/// <param name="buttonMoveUp"></param>
		/// <param name="buttonMoveDown"></param>
		/// <param name="buttonRemove"></param>
		public void SelectionChangeManageButtonStates(Button buttonMoveUp, Button buttonMoveDown, Button buttonRemove)
		{
			bool enableMoveUp;
			bool enableMoveDown;
			bool enableRemove;

			SelectionChangeManageButtonStates(out enableMoveUp, out enableMoveDown, out enableRemove);

			buttonMoveUp.Enabled = enableMoveUp;
			buttonMoveDown.Enabled = enableMoveDown;
			buttonRemove.Enabled = enableRemove;
		}

		/// <summary>
		/// Provides bool values for enabling or disabling controls based on the current selection.
		/// </summary>
		/// <param name="enableMoveUp"></param>
		/// <param name="enableMoveDown"></param>
		/// <param name="enableRemove"></param>
		public void SelectionChangeManageButtonStates(out bool enableMoveUp, out bool enableMoveDown, out bool enableRemove)
		{
			enableMoveUp = false;
			enableMoveDown = false;
			enableRemove = false;

			List<int> selectedIndexes = GetSelectedIndexes();
			int selectionCount = selectedIndexes.Count;

			if (selectionCount == 0)
			{
				return;
			}

			enableRemove = true;

			int itemCount = base.Items.Count;
			if (itemCount < 2)
			{
				return;
			}

			int index = selectedIndexes[0];
			if (selectionCount == 1 && index == 0)
			{
				enableMoveDown = true;
			}
			else if (selectionCount == 1 && index == itemCount - 1)
			{
				enableMoveUp = true;
			}
			else
			{
				enableMoveUp = true;
				enableMoveDown = true;
			}
		}
	}
}
