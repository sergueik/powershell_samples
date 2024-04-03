using System;
using System.Collections.Generic;
using System.Text;

namespace Yaml
{
    public class YamlCharacter
    {

        private const string INDICATORS = "-:[]{},?*&!|#@%^'\"";
        private const string INDICATORS_SP = "-:";
        private const string INDICATORS_INLINE = "[]{},";
        private const string INDICATORS_SIMPLE = ":[]{},";
        private const string INDICATORS_NONSP = "?*&!]|#@%^\"'";
        public static bool @Is(char c, YamlCharacterType type)
        {
            switch (type)
            {
                case YamlCharacterType.Printable: return IsPrintableChar(c);
                case YamlCharacterType.Word: return IsWordChar(c);
                case YamlCharacterType.Line: return IsLineChar(c);
                case YamlCharacterType.LineSP: return IsLineSpChar(c);
                case YamlCharacterType.Space: return IsSpaceChar(c);
                case YamlCharacterType.LineBreak: return IsLineBreakChar(c);
                case YamlCharacterType.Digit: return Char.IsDigit(c);
                case YamlCharacterType.Indent: return (c == ' ');                
                default: return false;

            }

        }

        public static bool IsLineBreakChar(char c)
        {
            if (c == 10 || c == 13 || c == 0x85 || c == 0x2028 || c == 0x2029) return true;
            return false;
        }

        public static bool IsSpaceChar(char c)
        {
            if (c == 9 || c == 0x20) return true;
            return false;
        }

        public static bool IsLineSpChar(char c)
        {
            if (c == 10 || c == 13 || c == 0x85) return false;
            return IsPrintableChar(c);
        }

        public static bool IsWordChar(char c)
        {
            if (c >= 0x41 && c <= 0x5a) return true;
            if (c >= 0x61 && c <= 0x7a) return true;
            if (c >= 0x30 && c <= 0x39) return true;
            if (c == '-') return true;
            return false;
        }

        public static bool IsPrintableChar(char c)
        {
            if (c >= 0x20 && c <= 0x7e) return true;
            if (c == 9 || c == 10 || c == 13 || c == 0x85) return true;
            if (c >= 0xa0 && c <= 0xd7ff) return true;
            if (c >= 0xe000 && c <= 0xfffd) return true;                        
            return false;
        }

        public static bool IsLineChar(char c)
        {
            if (c == 0x20 || c == 9 || c == 10 || c == 13 || c == 0x85) return false;
            return IsPrintableChar(c);
        }

        public static bool @Is(int c, YamlCharacterType type)
        {
            if (c == -1) return false;
            char ch = Convert.ToChar(c);
            return Is(ch, type);
        }

        public static bool IsIndicator(char c)
        {            
            return (INDICATORS.IndexOf(c) != -1);
        }

        public static bool IsIndicatorSpace(char c)
        {
            return (INDICATORS_SP.IndexOf(c) != -1);
        }

        public static bool IsIndicatorInline(char c)
        {
            return (INDICATORS_INLINE.IndexOf(c) != -1);
        }
        
        public static bool IsIndicatorNonSpace(char c)
        {
            return (INDICATORS_NONSP.IndexOf(c) != -1);
        }

        public static bool IsIndicatorSimple(char c)
        {
            return (INDICATORS_SIMPLE.IndexOf(c) != -1);
        }

    }
}
