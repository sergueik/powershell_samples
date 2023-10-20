using HKS.FolderMetadata.Extensions;
using HKS.FolderMetadata.Generic;
using System;
using System.IO;

namespace HKS.FolderMetadata.Configuration
{
	/// <summary>
	/// ArgParser parses the arguments specified by the command line.
	/// </summary>
	public class ArgParser
	{
		/// <summary>
		/// Parses the command line arguments provided at startup of the application.
		/// </summary>
		public ArgParser()
		{
			Initialize();

			string[] args = Environment.GetCommandLineArgs();
			if (ArrayHelper.IsNullOrEmpty(args))
			{
				return;
			}

			int length = args.Length;
			int lastIndex = length - 1;

			for (int i = 0; i < length; i++)
			{
				string arg = args[i];
				if (!IsParameterName(arg))
				{
					continue;
				}

				ParamConstants paramType = ParameterNames.ParseParamConstants(arg);

				if (SetStandaloneParameter(paramType))
				{
					continue;
				}
				else if (i == lastIndex) //Parameter without value
				{
					continue;
				}
				
				string value = args[i + 1];
				if (SetValueParameter(paramType, value))
				{
					i++;
				}
			}

			this.ShowHelp = (this.ShowHelp || string.IsNullOrEmpty(this.FolderPath));
		}

		/// <summary>
		/// Initializes all properties.
		/// </summary>
		private void Initialize()
		{
			this.FolderPath = null;
			this.Language = null;
			this.Register = false;
			this.Unregister = false;
			this.ShowHelp = true;
			this.ConfigurationMode = Dialogs.FrmMain.ConfigureMode.None;
		}

		/// <summary>
		/// Checks if the specified argument is a parameter name (e.g. /dir).
		/// </summary>
		/// <param name="arg">The argument to check.</param>
		/// <returns>Returns true if it is a parameter name, otherwise false.</returns>
		private bool IsParameterName(string arg)
		{
			return arg != null && arg.StartsWith("/", StringComparison.InvariantCulture);
		}

		/// <summary>
		/// Sets properties for parameters that do not require a value.
		/// </summary>
		/// <param name="paramType">The parameter to set.</param>
		/// <returns>Returns true if it is a standalone parameter, otherwise false.</returns>
		private bool SetStandaloneParameter(ParamConstants paramType)
		{
			switch (paramType)
			{
				case ParamConstants.Help:
					this.ShowHelp = true;
					break;
				case ParamConstants.Register:
					this.Register = true;
					this.ShowHelp = false;
					break;
				case ParamConstants.Unregister:
					this.Unregister = true;
					this.ShowHelp = false;
					break;
				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Sets the property for a parameter requiring a value.
		/// </summary>
		/// <param name="paramType">The parameter to set.</param>
		/// <param name="value">The value for the specified parameter</param>
		/// <returns>Returns true if the parameter requires a value, otherwise false.</returns>
		private bool SetValueParameter(ParamConstants paramType, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}

			switch (paramType)
			{
				case ParamConstants.Lang:
					this.Language = value.Trim();
					break;
				case ParamConstants.Dir:
					SetFolder(value);
					break;
				case ParamConstants.Configure:
					SetConfigurationMode(value);
					break;
				default:
					return false;
			}

			return true;
		}

		/// <summary>
		/// Sets the configuration mode
		/// </summary>
		/// <param name="value">The configuratin mode value</param>
		private void SetConfigurationMode(string value)
		{
			value = value.Trim();

			///<seealso cref="Extensions.String"/>
			if (value.IsAnyValue("author", "authors"))
			{
				this.ConfigurationMode = Dialogs.FrmMain.ConfigureMode.Authors;
			}
			else if (value.IsAnyValue("tag", "tags"))
			{
				this.ConfigurationMode = Dialogs.FrmMain.ConfigureMode.Tags;
			}
		}

		/// <summary>
		/// Sets the folder info if the path exists.
		/// </summary>
		/// <param name="arg">The folder path to set.</param>
		private void SetFolder(string arg)
		{
			try
			{
				DirectoryInfo dirInfo = new DirectoryInfo(arg);
				string fullPath = dirInfo.FullName;
				if (dirInfo.Exists &&
				File.GetAttributes(fullPath).HasFlag(FileAttributes.Directory))
				{
					this.FolderPath = fullPath;
					this.ShowHelp = false;
				}
			}
			catch
			{ }
		}

		/// <summary>
		/// Returns the mode for configuration of entries (i.e. None, Authors, or Tags).
		/// </summary>
		public Dialogs.FrmMain.ConfigureMode ConfigurationMode { get; private set; }

		/// <summary>
		/// Returns the full path to the folder to configure.
		/// </summary>
		public string FolderPath { get; private set; }

		/// <summary>
		/// Returns the language (e.g. "de-DE", "zh-TW"), or "select" to select a supported language from a list.
		/// </summary>
		public string Language { get; private set; }

		/// <summary>
		/// Returns if the application should register the context menu entry.
		/// </summary>
		public bool Register { get; private set; }

		/// <summary>
		/// Returns if the help dialog should show.
		/// </summary>
		public bool ShowHelp { get; private set; }

		/// <summary>
		/// Returns if the application should unregister the context menu entry
		/// </summary>
		public bool Unregister { get; private set; }
	}
}
