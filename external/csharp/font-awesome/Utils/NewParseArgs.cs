using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utils {
// based on https://github.com/sergueik/powershell_samples/blob/master/csharp/pbkdf2-csharp/Utils/ParseArgs.cs
	// NOTE: cannot use space in named match groups:
	// "short option value"
	// will only throw in runtime: 
	// System.ArgumentException: parsing "^(--(?<long>[A-Za-z0-9\-]+)(=(?<lvalue>.*))?)|(-(?<short option value>[A-Za-z0-9])(?<svalue>.*)?)$" - Invalid group name: Group names must begin with a word character.

    public static class NewParseArgs {
        private static readonly Regex rx =
            new Regex(@"^(--(?<long>[A-Za-z0-9\-]+)(=(?<lvalue>.*))?)|(-(?<short>[A-Za-z0-9])(?<svalue>.*)?)$",
                      RegexOptions.Compiled);

        public static Dictionary<string, string> Parse(string[] args)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                var a = args[i];
                var m = rx.Match(a);
                if (!m.Success)
                    continue;

                string key = null;
                string value = null;

                if (m.Groups["long"].Success)
                {
                    key = m.Groups["long"].Value;
                    if (m.Groups["lvalue"].Success && m.Groups["lvalue"].Length > 0)
                        value = m.Groups["lvalue"].Value;
                }
                else if (m.Groups["short"].Success)
                {
                    key = m.Groups["short"].Value;
                    if (m.Groups["svalue"].Success && m.Groups["svalue"].Length > 0)
                        value = m.Groups["svalue"].Value;
                }

                // If still no lvalue and next arg is non-option, treat it as lvalueue
                if (value == null && i + 1 < args.Length)
                {
                    var next = args[i + 1];
                    if (!next.StartsWith("-"))
                    {
                        value = next;
                        i++;
                    }
                }

                // Boolean option (switch)
                if (value == null)
                    value = "true";

                if (!result.ContainsKey(key))
                    result.Add(key, value);
            }

            return result;
        }
    }
}

