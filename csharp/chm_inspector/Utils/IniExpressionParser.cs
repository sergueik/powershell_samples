using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

public static class IniExpressionParser
{
    // Example: "STGM_READ | STGM_SHARE_DENY_NONE"
    public static uint ParseEnumExpression<TEnum>(string expr)
        where TEnum : struct, Enum {
        if (string.IsNullOrWhiteSpace(expr))
            throw new ArgumentNullException(nameof(expr));

        uint result = 0;
	// handle expressions like STGM_READ | STGM_SHARE_DENY_NONE
        string[] tokens = expr.Split('|', StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens) {
            string trimmed = token.Trim();

            if (trimmed.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                // Allow explicit hex
                result |= Convert.ToUInt32(trimmed, 16);
            } else if (uint.TryParse(trimmed, out uint val)) {
                result |= val;
            } else if (Enum.TryParse(typeof(TEnum), trimmed, ignoreCase: true, out object? parsed)) {
                result |= Convert.ToUInt32(parsed);
            } else {
                throw new FormatException(String.Format("Invalid flag value: {0}", trimmed ));
            }
        }

        return result;
    }
}

