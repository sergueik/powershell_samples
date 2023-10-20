using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HKS.FolderMetadata.Configuration
{
	/// <summary>
	///  The Registrator class registers or unregisters this application for use with folders.
	///  The registration is done by creating a sub-key under the HKEY_CLASSES_ROOT\Folder registry key.
	///  The language can be either used from the system or by specifying the /lang parameter in the command line.
	/// </summary>
	public class Registrator
	{
		private RegistryKey KEY_FOLDER = null;
		private readonly string MY_PATH = System.Reflection.Assembly.GetExecutingAssembly().Location;
		private readonly System.Globalization.CultureInfo CURRENT_CULTURE = System.Globalization.CultureInfo.CurrentCulture;

		/// <summary>
		/// Constructor
		/// </summary>
		public Registrator()
		{
			try
			{
				KEY_FOLDER = Registry.ClassesRoot.OpenSubKey("Folder", true);
				this.CanRegister = true;
			}
			catch
			{
				MessageBox.Show(Strings.Registrator_Error_UnableToOpenRegistryForWriting, Strings.Registrator_RegistryAccessError, MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.CanRegister = false;
			}
		}

		/// <summary>
		/// Returns if the current user can modify the registry. This property is set in the constructor.
		/// </summary>
		public bool CanRegister { get; private set; }

		/// <summary>
		/// Creates a context-menu for folders if the user has the proper rights (run as Administrator).
		/// </summary>
		public void Register()
		{
			if (!this.CanRegister)
			{
				return;
			}

			Unregister();

			RegistryKey keyShell = KEY_FOLDER.OpenSubKey("shell", true);
			RegistryKey keyContextMenu = OpenOrCreateKey(keyShell,Strings.Registrator_ContextMenu);
			RegistryKey keyCommand = OpenOrCreateKey(keyContextMenu, "command");
			string path = MY_PATH;
			if (path.Contains(" "))
			{
				path = "\"" + path + "\"";
			}
			path = path + " /dir \"%1\"";
			keyCommand.SetValue("", path);
		}

		/// <summary>
		/// Removes the context menu for folders by checking the file location in the default value of the "command" registry key.
		/// </summary>
		public void Unregister()
		{
			if (!this.CanRegister)
			{
				return;
			}

			RegistryKey shellSubKey = KEY_FOLDER.OpenSubKey("shell", true);
			List<string> mySubKeys = GetUnregisterSubKeyNames(shellSubKey);
			Unregister(shellSubKey, mySubKeys);
		}

		private RegistryKey OpenOrCreateKey(RegistryKey parent, string keyName)
		{
			RegistryKey retVal = parent.OpenSubKey(keyName, true);
			if (retVal == null)
			{
				retVal = parent.CreateSubKey(keyName);
			}

			return retVal;
		}

		/// <summary>
		/// Retrieves the names of all key names underneath HKEY_CLASSES_ROOT\Folder\shell.
		/// </summary>
		/// <param name="shellSubKey">The key from which to retrieve the names</param>
		/// <returns>Retrieves a list of key names.</returns>
		private List<string> GetUnregisterSubKeyNames(RegistryKey shellSubKey)
		{
			string[] subKeyNames = shellSubKey.GetSubKeyNames();
			List<string> retVal = new List<string>();

			foreach (string subKeyName in subKeyNames)
			{
				if (IsMySubKey(shellSubKey, subKeyName))
				{
					retVal.Add(subKeyName);
				}
			}

			return retVal;
		}

		private void Unregister(RegistryKey shellSubKey, List<string> mySubKeys)
		{
			List<string> notDeletedSubKeys = new List<string>();
			foreach (string mySubKey in mySubKeys)
			{
				Unregister(shellSubKey, mySubKey, notDeletedSubKeys);
			}

			if (notDeletedSubKeys.Count > 0)
			{
				string ndsk = string.Join("\r\n", notDeletedSubKeys.ToArray());
				string message = string.Format(Strings.Registrator_CouldNotDelete, "HKEY_CLASSES_ROOT\\Folder", ndsk);
				MessageBox.Show(message, Strings.Registrator_UnregisterError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Deletes the registry key for the context menu and all sub-keys.
		/// </summary>
		/// <param name="shellSubKey">The parent key</param>
		/// <param name="mySubKey">The key to delete</param>
		/// <param name="notDeletedSubKeys">Adds keys that could not be deleted to this list.</param>
		private void Unregister(RegistryKey shellSubKey, string mySubKey, List<string> notDeletedSubKeys)
		{
			try
			{
				shellSubKey.DeleteSubKeyTree(mySubKey);
			}
			catch
			{
				notDeletedSubKeys.Add(mySubKey);
			}
		}

		/// <summary>
		/// Identifies if the specified sub-key contains a reference to this application by checking the default value of the "command" sub-key.
		/// </summary>
		/// <param name="shellSubKey"></param>
		/// <param name="parentKeyName"></param>
		/// <returns></returns>
		private bool IsMySubKey(RegistryKey shellSubKey, string parentKeyName)
		{
			RegistryKey parentKey = shellSubKey.OpenSubKey(parentKeyName, false);
			RegistryKey keyCommand = parentKey.OpenSubKey("command");
			if (keyCommand == null)
			{
				return false;
			}
			string path = keyCommand.GetValue("", null) as string;
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			path = path.TrimStart('"',' ', '\t');
			return path.StartsWith(MY_PATH, true, CURRENT_CULTURE);
		}
	}
}
