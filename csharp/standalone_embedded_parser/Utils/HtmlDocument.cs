using System;
using System.Collections.Generic;
using System.Text;

namespace Utils {
    public class HtmlDocument {
        public HtmlNode DocumentElement;

        public HtmlDocument() {
            DocumentElement = new HtmlNode("document");
        }

        public void LoadHtml(string html) {
            var parser = new HtmlParser();
            // Console.Error.WriteLine("{0}", "LoadHtml"); 
            
            DocumentElement = parser.Parse(html);
        }

        public IEnumerable<HtmlNode> GetElementsByTagName(string tag) {
            return DocumentElement.GetElementsByTagName(tag);
        }
    }

    public class HtmlNode
    {
        public string TagName;
        public string InnerText = "";
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public List<HtmlNode> Children = new List<HtmlNode>();
        public HtmlNode Parent;

        public HtmlNode(string tag)
        {
            TagName = tag;
        }

        public void AddChild(HtmlNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public string GetAttribute(string name)
        {
            string v;
            return Attributes.TryGetValue(name, out v) ? v : null;
        }

        public IEnumerable<HtmlNode> GetElementsByTagName(string tag)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var c = Children[i];
                // Console.Error.WriteLine("{0}", c.TagName ); 
                if (string.Compare(c.TagName, tag, true) == 0)
                    yield return c;

                foreach (var sub in c.GetElementsByTagName(tag))
                    yield return sub;
            }
        }
    
	    public IEnumerable<HtmlNode> QuerySelectorAll(string selector){
		    return CssSelectorEngine.Select(this, selector);
		}

    }

    internal class HtmlParser
    {
        private string _text;
        private int _pos;
        private int _length;

        public HtmlNode Parse(string html)
        {
            _text = html ?? "";
            _pos = 0;
            _length = _text.Length;

            HtmlNode root = new HtmlNode("document");
                // Console.Error.WriteLine("root");

            while (_pos < _length)
            {
                // Console.Error.WriteLine("parsing at {0}", _pos);
                var node = ParseNode();
                if (node != null)
                    root.AddChild(node);
            }

            return root;
        }

        private HtmlNode ParseNode() {
            SkipWhitespace();

            if (_pos >= _length) return null;
// Console.Error.WriteLine("ParseNode {0}", _text[_pos]);

            if (_text[_pos] == '<') {
                // Tag
                return ParseElement();
            } else {
                // Text
                var text = ParseText();
                HtmlNode tn = new HtmlNode("#text");
                tn.InnerText = text;
                return tn;
            }
        }

        private HtmlNode ParseElement()
        {
        	

            if (_text[_pos] != '<') return null;

            _pos++; // skip <
            string tag = ReadTagName();

        	// // Console.Error.WriteLine("ParseElement \"{0}\"", tag);
            HtmlNode node = new HtmlNode(tag);

            // Attributes
            while (true)
            {
                SkipWhitespace();
                if (_pos >= _length) break;

                if (_text[_pos] == '>' || _text[_pos] == '/')
                    break;

                var attr = ReadAttribute();
                if (attr != null)
                    node.Attributes[attr.Item1] = attr.Item2;
            }

            // Self closing?
            if (Match("/>"))
            {
                _pos += 2;
                return node;
            }

            // Normal end of start tag
            if (Match(">"))
                _pos++;

            // Parse children until closing tag
            while (!Match("</" + tag, StringComparison.OrdinalIgnoreCase))
            {
                if (_pos >= _length) break;

                var child = ParseNode();
                if (child == null) break;

                node.AddChild(child);
            }

            // Skip the closing tag
            if (Match("</"))
            {
                _pos += 2;
                ReadTagName();
                SkipUntil('>');
                if (_pos < _length) _pos++;
            }

            return node;
        }

        private string ParseText()
        {
            int start = _pos;
            while (_pos < _length && _text[_pos] != '<')
                _pos++;

            return _text.Substring(start, _pos - start).Trim();
        }

        private string ReadTagName()
        {
            SkipWhitespace();
            int start = _pos;

            while (_pos < _length)
            {
                char c = _text[_pos];
                if (char.IsLetterOrDigit(c) || c == '-' || c == ':' || c == '_')
                {
                    _pos++;
                }
                else break;
            }

            return _text.Substring(start, _pos - start);
        }

        private Tuple<string, string> ReadAttribute()
        {
            SkipWhitespace();

            string name = ReadTagName();
            if (name == "") return null;

            SkipWhitespace();

            string value = "";

            if (Match("="))
            {
                _pos++;
                SkipWhitespace();
                value = ReadAttributeValue();
            }

            return Tuple.Create(name, value);
        }

        private string ReadAttributeValue()
        {
            if (_pos >= _length) return "";

            if (_text[_pos] == '"' || _text[_pos] == '\'')
            {
                char quote = _text[_pos];
                _pos++;

                int start = _pos;
                while (_pos < _length && _text[_pos] != quote)
                    _pos++;

                string v = _text.Substring(start, _pos - start);
                if (_pos < _length) _pos++;
                return v;
            }

            // Unquoted
            int s = _pos;
            while (_pos < _length && !Char.IsWhiteSpace(_text[_pos]) &&
                   _text[_pos] != '>' && _text[_pos] != '/')
            {
                _pos++;
            }

            return _text.Substring(s, _pos - s);
        }

        private void SkipWhitespace()
        {
            while (_pos < _length && Char.IsWhiteSpace(_text[_pos]))
                _pos++;
        }

        private bool Match(string s)
        {
            if (_pos + s.Length > _length) return false;

            for (int i = 0; i < s.Length; i++)
                if (_text[_pos + i] != s[i]) return false;

            return true;
        }

        private bool Match(string s, StringComparison cmp)
        {
            if (_pos + s.Length > _length) return false;

            return string.Compare(_text, _pos, s, 0, s.Length, cmp) == 0;
        }

        private void SkipUntil(char c)
        {
            while (_pos < _length && _text[_pos] != c)
                _pos++;
        }
    }
    
    internal static class CssSelectorEngine
{
    public static IEnumerable<HtmlNode> Select(HtmlNode root, string selector)
    {
        selector = selector.Trim();

        // Descendant selector: "div span"
        int space = selector.IndexOf(' ');
        if (space > 0)
        {
            string first = selector.Substring(0, space).Trim();
            string rest = selector.Substring(space + 1).Trim();

            foreach (var node in Select(root, first))
            {
                foreach (var n in Select(node, rest))
                    yield return n;
            }
            yield break;
        }

        // Simple selectors (tag, #id, .class, tag.class, tag#id)
        string tag = null;
        string id = null;
        string cls = null;
        string attrName = null;
        string attrEquals = null;
        string attrContains = null;

        // Parse tag
        int pos = 0;
        while (pos < selector.Length &&
               (char.IsLetterOrDigit(selector[pos]) || selector[pos] == '-' || selector[pos] == '_'))
        {
            pos++;
        }
        if (pos > 0)
            tag = selector.Substring(0, pos);

        // Parse the remainder: .class, #id, [attr]
        while (pos < selector.Length)
        {
            if (selector[pos] == '#')
            {
                pos++;
                int s = pos;
                while (pos < selector.Length && selector[pos] != '.' && selector[pos] != '[')
                    pos++;
                id = selector.Substring(s, pos - s);
            }
            else if (selector[pos] == '.')
            {
                pos++;
                int s = pos;
                while (pos < selector.Length && selector[pos] != '#' && selector[pos] != '[')
                    pos++;
                cls = selector.Substring(s, pos - s);
            }
            else if (selector[pos] == '[')
            {
                pos++;
                int s = pos;
                while (pos < selector.Length && selector[pos] != '=' && selector[pos] != ']' && selector[pos] != '~')
                    pos++;
                attrName = selector.Substring(s, pos - s);

                if (pos < selector.Length && selector[pos] == '=')
                {
                    pos++;
                    int v = pos;
                    while (pos < selector.Length && selector[pos] != ']')
                        pos++;
                    attrEquals = selector.Substring(v, pos - v).Trim('"');
                }
                else if (pos < selector.Length && selector[pos] == '~')
                {
                    pos++;
                    if (pos < selector.Length && selector[pos] == '=')
                    {
                        pos++;
                        int v = pos;
                        while (pos < selector.Length && selector[pos] != ']')
                            pos++;
                        attrContains = selector.Substring(v, pos - v).Trim('"');
                    }
                }

                if (pos < selector.Length && selector[pos] == ']')
                    pos++;
            }
            else
            {
                pos++;
            }
        }

        // Walk the tree manually (no LINQ)
        var stack = new Stack<HtmlNode>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            HtmlNode node = stack.Pop();

            // Check match:
            if (Matches(node, tag, id, cls, attrName, attrEquals, attrContains))
                yield return node;

            // Add children to stack
            for (int i = 0; i < node.Children.Count; i++)
                stack.Push(node.Children[i]);
        }
    }

    private static bool Matches(HtmlNode node, string tag, string id, string cls,
                                string attrName, string attrEquals, string attrContains)
    {
        // Tag match
        if (tag != null && !Same(node.TagName, tag))
            return false;

        // ID match
        if (id != null)
        {
            string v = node.GetAttribute("id");
            if (v == null || v != id)
                return false;
        }

        // Class match
        if (cls != null)
        {
            string v = node.GetAttribute("class");
            if (v == null) return false;

            // class token contains
            if (!ContainsToken(v, cls))
                return false;
        }

        if (attrName != null)
        {
            string v = node.GetAttribute(attrName);
            if (v == null)
                return false;

            if (attrEquals != null && v != attrEquals)
                return false;

            if (attrContains != null && !ContainsToken(v, attrContains))
                return false;
        }

        return true;
    }

    private static bool ContainsToken(string list, string token)
    {
        string[] parts = list.Split(' ');
        for (int i = 0; i < parts.Length; i++)
            if (Same(parts[i], token))
                return true;
        return false;
    }

    private static bool Same(string a, string b)
    {
        return string.Compare(a, b, true) == 0;
    }
}

}

