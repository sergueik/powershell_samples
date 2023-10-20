
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Utils {
	public class Arguments {
		// https://learn.microsoft.com/en-us/dotnet/api/system.collections.specialized.hybriddictionary?view=net-7.0
		private HybridDictionary parameters;

		public Arguments(string[] args) {
			
			parameters = new HybridDictionary();
			var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			string parameter = null;
			string[] parts;

			foreach (string arg in args) {
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				parts = spliter.Split(arg, 3);
				switch (parts.Length) {

					case 1:
						if (parameter != null) {
							if (!parameters.Contains(parameter)) {
								parts[0] = remover.Replace(parts[0], "$1");
								parameters.Add(parameter, parts[0]);
							}
							parameter = null;
						}

						break;
					case 2:
						if (parameter != null) {
							if (!parameters.Contains(parameter))
								parameters.Add(parameter, "true");
						}
						parameter = parts[1];
						break;
				
					case 3:
				
						if (parameter != null) {
							if (!parameters.Contains(parameter))
								parameters.Add(parameter, "true");
						}
						parameter = parts[1];
						
						if (!parameters.Contains(parameter)) {
							parts[2] = remover.Replace(parts[2], "$1");
							parameters.Add(parameter, parts[2]);
						}
						parameter = null;
						break;
				}
			}

			if (parameter != null) {
				if (!parameters.Contains(parameter))
					parameters.Add(parameter, "true");
			}
		}

		public object this[string name] {
			get {
				return (parameters[name]);
			}
		}
	}
}
