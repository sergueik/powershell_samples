using HKS.FolderMetadata.Extensions;
using System;

namespace HKS.FolderMetadata.Configuration
{
	/// <summary>
	/// Enumeration for parameter parsing.
	/// </summary>
	/// <seealso cref="ParseParamConstants"/>
	public enum ParamConstants { Unknown, Help, Register, Unregister, Lang, Dir, Configure }

	/// <summary>
	/// The class contains constants with the supported parameter names
	/// </summary>
	public static class ParameterNames
	{
		public const string REGISTER = "register";
		public const string UNREGISTER = "unregister";
		public const string LANGUAGE = "lang";
		public const string FOLDER = "dir";
		public const string CONFIGURE = "configure";
		public const string HELP = "?";

		/// <summary>
		/// Parses an argument into a <see cref="ParamConstants"/> value
		/// </summary>
		/// <param name="arg">The command line argument to parse</param>
		/// <returns>Returns a ParamConstants value matching the argument or Unknown.</returns>
		public static ParamConstants ParseParamConstants(string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				return ParamConstants.Unknown;
			}
			else if (arg.StartsWith("/"))
			{
				arg = arg.Substring(1);
			}

			///<see cref="Extensions.String"/>
			if (arg.IsEmpty())
			{
				return ParamConstants.Unknown;
			}
			else if (ParameterNames.HELP.Equals(arg, StringComparison.InvariantCulture))
			{
				return ParamConstants.Help;
			}

			Type enumType = typeof(ParamConstants);
			string[] names = Enum.GetNames(enumType);
			foreach (string name in names)
			{
				if (name.Equals(arg, StringComparison.InvariantCultureIgnoreCase))
				{
					return (ParamConstants)Enum.Parse(enumType, name);
				}
			}

			return ParamConstants.Unknown;
		}
	}
}
