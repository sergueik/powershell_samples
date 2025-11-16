using System;
using System.Collections.Generic;

namespace Utils {
    public static class IniExpressionParser {
        public static uint ParseEnumFlags<TEnum>(string expr)
            where TEnum : struct {
            if (string.IsNullOrWhiteSpace(expr))
                throw new ArgumentException("Expression is empty", "expr");

            uint result = 0;
            string[] tokens = expr.Split('|');
            foreach (string raw in tokens) {
                string token = raw.Trim();
                uint numeric;
                TEnum value;
                if (token.Length == 0)
                    continue;

                if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) {
                    result |= Convert.ToUInt32(token, 16);
                } else if (Enum.TryParse<TEnum>(token, true, out value)) {
                    result |= Convert.ToUInt32(value);
                } else if (UInt32.TryParse(token, out numeric)) {
                    result |= numeric;
                } else {
                    throw new ArgumentException("Unknown flag: " + token);
                }
            }
            return result;
        }
    }
}

