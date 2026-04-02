using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils {
	public class EnvVars {
		
		private static string result = null;
		private static readonly Regex regex =
            new Regex(@"\$(?:\{(?:env:)?(\w+)\}|(\w+))",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static string ResolveEnvVars(string input) {
			if (input == null)
				return null;

			return regex.Replace(input, (Match match) => {
				string varName = match.Groups[1].Success
                    ? match.Groups[1].Value
                    : match.Groups[2].Value;
				// TODO: Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
				string value = Environment.GetEnvironmentVariable( varName, EnvironmentVariableTarget.User) ;
				if (value == null)
					value =  Environment.GetEnvironmentVariable( varName,  EnvironmentVariableTarget.Process);	
				return value == null ? string.Empty : value;
			});
		}
	}
}
