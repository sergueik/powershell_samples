using HKS.FolderMetadata.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HKS.FolderMetadata.Generic
{
	/// <summary>
	/// The Metadata class provides methods for reading and writing to desktop.ini.
	/// </summary>
	public class Metadata
	{
		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

		/// <summary>
		/// The name of the section in desktop.ini
		/// </summary>
		private const string SECTION = "{F29F85E0-4FF9-1068-AB91-08002B27B3D9}";

		/// <summary>
		/// The setting name in desktop.ini for the title.
		/// </summary>
		private const string TITLE = "Prop2";

		/// <summary>
		/// The setting name in desktop.ini for the subject.
		/// </summary>
		private const string SUBJECT = "Prop3";

		/// <summary>
		/// The setting name in desktop.ini for the authors.
		/// </summary>
		private const string AUTHORS = "Prop4";

		/// <summary>
		/// The setting name in desktop.ini for the tags.
		/// </summary>
		private const string TAGS = "Prop5";

		/// <summary>
		/// The setting name in desktop.ini for the comment.
		/// </summary>
		private const string COMMENT = "Prop6";

		/// <summary>
		/// The full path to desktop.ini.
		/// </summary>
		private readonly string FULL_PATH = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="directory">The directory to configure.</param>
		public Metadata(string directory)
		{
			if (!string.IsNullOrEmpty(directory))
			{
				this.FULL_PATH = Path.Combine(directory, "desktop.ini");
			}

			this.Title = null;
			this.Subject = null;
			this.Authors = null;
			this.Comment = null;
			this.Tags = null;
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the authors.
		/// </summary>
		public List<string> Authors { get; set; }

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		public List<string> Tags { get; set; }

		/// <summary>
		/// Returns if the full path to desktop.ini has been specified.
		/// </summary>
		public bool HasFilePath
		{
			get
			{
				return !string.IsNullOrEmpty(this.FULL_PATH);
			}
		}

		/// <summary>
		/// Returns if there are any data to save.
		/// </summary>
		private bool HasData
		{
			get
			{
				return
					!string.IsNullOrEmpty(this.Title) ||
					!string.IsNullOrEmpty(this.Subject) ||
					!string.IsNullOrEmpty(this.Comment) ||
					!ListHelper.IsNullOrEmpty(this.Authors) ||
					!ListHelper.IsNullOrEmpty(this.Tags);
			}
		}

		/// <summary>
		/// Loads the metadata from desktop.ini.
		/// </summary>
		public void Load()
		{
			if (!HasFilePath)
			{
				return;
			}

			this.Title = ReadPropertyValue(TITLE);
			this.Subject = ReadPropertyValue(SUBJECT);
			this.Authors = ReadPropertyListValues(AUTHORS);
			this.Tags = ReadPropertyListValues(TAGS);
			this.Comment = ReadPropertyValue(COMMENT);
		}

		/// <summary>
		/// Saves the metadata to desktop.ini or removes the section from desktop.ini if no metadata are present.
		/// </summary>
		public void Save()
		{
			if (!HasFilePath)
			{
				return;
			}
			
			if (HasData)
			{
				SaveData();
			}
			else
			{
				DeleteSection();
			}

			SetFileAttributes();
		}


		private void SaveData()
		{
			WritePropertyValue(TITLE, this.Title);
			WritePropertyValue(SUBJECT, this.Subject);
			WritePropertyListValues(AUTHORS, this.Authors);
			WritePropertyListValues(TAGS, this.Tags);
			WritePropertyValue(COMMENT, this.Comment);
		}

		private void DeleteSection()
		{
			WritePrivateProfileString(SECTION, null, null, this.FULL_PATH); //Deletes the whole section
		}

		/// <summary>
		/// Sets the file attributes System and Hidden for desktop.ini.
		/// </summary>
		private void SetFileAttributes()
		{
			File.SetAttributes(this.FULL_PATH, FileAttributes.System | FileAttributes.Hidden);
		}

		/// <summary>
		/// Reads the value of the property without the "31," prefix.
		/// </summary>
		/// <param name="property">The property for which to retrieve the value.</param>
		/// <returns>Returns the property value or null.</returns>
		private string ReadPropertyValue(string property)
		{
			StringBuilder sb = new StringBuilder(65536);
			GetPrivateProfileString(SECTION, property, null, sb, sb.Capacity, this.FULL_PATH);
			if (StringBuilderHelper.IsNullOrEmpty(sb)) //Property does not exist in desktop.ini
			{
				return null;
			}

			//Example for Title: Prop2=31,Hello world!
			string value = sb.ToString();
			int indexComma = value.IndexOf(',');
			if (indexComma < 0)
			{
				return value;
			}

			string prefix = value.Substring(0, indexComma);
			prefix = prefix.Replace(" ", "");
			prefix = prefix.Replace("\t", "");
			if (prefix == "31") //This is the expected case
			{
				return value.Substring(indexComma + 1);
			}

			return value;
		}

		/// <summary>
		/// Reads the value of a property and converts it to a list.
		/// </summary>
		/// <param name="property">The property for which to retrieve the values.</param>
		/// <returns>Returns a list of values or null.</returns>
		private List<string> ReadPropertyListValues(string property)
		{
			string value = ReadPropertyValue(property);
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			///<see cref="Extensions.String"/>
			return value.ToList();
		}

		/// <summary>
		/// Writes the property value or removes it from desktop.ini. If the value is null or empty, the property will be removed from desktop.ini
		/// </summary>
		/// <param name="property">The property to receive the value or to be removed.</param>
		/// <param name="value">The property value</param>
		private void WritePropertyValue(string property, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				value = null;
			}
			else
			{
				value = "31," + value;
			}

			WritePrivateProfileString(SECTION, property, value, this.FULL_PATH);
		}

		/// <summary>
		/// Writes the property value or removes it from desktop.ini. If the list is null or empty, or all its values are null or empty, the property will be removed from desktop.ini
		/// </summary>
		/// <param name="property">The property to receive the value or to be removed.</param>
		/// <param name="values">The property values</param>
		private void WritePropertyListValues(string property, List<string> values)
		{
			string value = ListHelper.ToString(values);
			WritePropertyValue(property, value);
		}
	}
}
