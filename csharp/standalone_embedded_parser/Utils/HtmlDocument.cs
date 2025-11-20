using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
	public class HtmlDocument {
		private string _text;
		private int _pos;
		private HtmlNode _documentElement;

		public HtmlNode DocumentElement {
			get { return _documentElement; }
		}

		public void LoadHtml(string html) {
			_text = html ?? "";
			_pos = 0;
			_documentElement = new HtmlNode("root");
			while (!EndOfText) {
				ParseNode(_documentElement);
			}
		}

		private void ParseNode(HtmlNode parent) {
			SkipWhitespace();

			if (LookAhead("<!--")) {
				SkipComment();
				return;
			}

			if (CurrentChar == '<') {
				HtmlNode elem = ParseElement();
				if (elem != null)
					parent.Children.Add(elem);
			} else {
				string text = ParseText();
				if (!string.IsNullOrEmpty(text)) {
					// #text child
					var textNode = new HtmlNode("#text");
					textNode.InnerText = text;
					parent.Children.Add(textNode);

					// Merge into parent's InnerText
					parent.InnerText += text;
				}
			}
		}

		private HtmlNode ParseElement() {
			if (CurrentChar != '<')
				return null;

			Advance(1); // skip '<'
			string tag = ParseTagName();
			var node = new HtmlNode(tag);

			// Parse attributes
			while (!EndOfText && CurrentChar != '>' && CurrentChar != '/') {
				SkipWhitespace();
				if (CurrentChar == '>' || CurrentChar == '/')
					break;
				ParseAttribute(node);
			}

			// Self-closing
			if (LookAhead("/>")) {
				Advance(2);
				return node;
			}

			if (CurrentChar == '>') {
				Advance(1);
				// Parse children recursively
				while (!EndOfText && !LookAhead("</" + tag)) {
					ParseNode(node);
				}
				// Skip closing tag
				if (LookAhead("</" + tag)) {
					Advance(2 + tag.Length);
					SkipUntil('>');
					Advance(1);
				}
			}

			return node;
		}

		private void ParseAttribute(HtmlNode node) {
			string name = ParseAttributeName();
			string value = "";

			SkipWhitespace();
			if (CurrentChar == '=') {
				Advance(1);
				SkipWhitespace();
				value = ParseAttributeValue();
			}

			node.Attributes[name] = value;
		}

		private void SkipComment() {
			Advance(4); // skip "<!--"
			while (!EndOfText) {
				if (LookAhead("-->")) {
					Advance(3);
					return;
				}
				Advance(1);
			}
			throw new Exception("Unterminated HTML comment");
		}

		private string ParseText() {
			int start = _pos;
			while (!EndOfText && CurrentChar != '<')
				Advance(1);
			return _text.Substring(start, _pos - start).Trim();
		}

		private string ParseTagName() {
			int start = _pos;
			while (!EndOfText && !Char.IsWhiteSpace(CurrentChar) && CurrentChar != '>' && CurrentChar != '/')
				Advance(1);
			return _text.Substring(start, _pos - start).ToLower();
		}

		private string ParseAttributeName() {
			int start = _pos;
			while (!EndOfText && !Char.IsWhiteSpace(CurrentChar) && CurrentChar != '=' && CurrentChar != '>' && CurrentChar != '/')
				Advance(1);
			return _text.Substring(start, _pos - start).ToLower();
		}

		private string ParseAttributeValue() {
			if (CurrentChar == '"' || CurrentChar == '\'') {
				char quote = CurrentChar;
				Advance(1);
				int start = _pos;
				while (!EndOfText && CurrentChar != quote)
					Advance(1);
				string val = _text.Substring(start, _pos - start);
				if (CurrentChar == quote)
					Advance(1);
				return val;
			} else {
				int start = _pos;
				while (!EndOfText && !Char.IsWhiteSpace(CurrentChar) && CurrentChar != '>')
					Advance(1);
				return _text.Substring(start, _pos - start);
			}
		}

		private void SkipUntil(char c) {
			while (!EndOfText && CurrentChar != c)
				Advance(1);
		}

		private void SkipWhitespace() {
			while (!EndOfText && Char.IsWhiteSpace(CurrentChar))
				Advance(1);
		}

		private bool LookAhead(string s) {
			if (_pos + s.Length > _text.Length)
				return false;
			for (int i = 0; i < s.Length; i++)
				if (_text[_pos + i] != s[i])
					return false;
			return true;
		}

		private void Advance(int n) {
			_pos += n;
		}

		private char CurrentChar {
			get { return _text[_pos]; }
		}

		private bool EndOfText {
			get { return _pos >= _text.Length; }
		}


		public IEnumerable<HtmlNode> GetElementsByTagName(string tagName) {
			return Traverse(_documentElement, tagName.ToLower());
		}

		private IEnumerable<HtmlNode> Traverse(HtmlNode node, string tag) {
			if (node.TagName == tag)
				yield return node;

			foreach (var child in node.Children) {
				foreach (var c in Traverse(child, tag))
					yield return c;
			}
		}
		// NOTE: does not work
		public IEnumerable<HtmlNode> QuerySelectorAll(string selector) {
			return _documentElement.Children[0].QuerySelectorAll(selector);
		}
	
	}
    
	public class HtmlNode {
		public string TagName;
		public string InnerText = "";
		public List<HtmlNode> Children = new List<HtmlNode>();
		public Dictionary<string,string> Attributes = new Dictionary<string,string>();

		public HtmlNode(string tagName) {
			this.TagName = tagName;
		}

		public string GetAttribute(string name) {
			string val;
			if (Attributes.TryGetValue(name.ToLower(), out val))
				return val;
			return null;
		}

		public IEnumerable<HtmlNode> QuerySelectorAll(string selector) {
		    if (string.IsNullOrEmpty(selector)) yield break;
		
		    selector = selector.Trim();
		    string tag = null;
		    string attrName = null;
		    string attrValue = null;
		
		    int startBracket = selector.IndexOf('[');
		    int endBracket = selector.IndexOf(']');
		
		    if (startBracket >= 0 && endBracket > startBracket) {
		        tag = selector.Substring(0, startBracket).Trim().ToLower();
		        string inside = selector.Substring(startBracket + 1, endBracket - startBracket - 1);
		        int eq = inside.IndexOf('=');
		        if (eq >= 0) {
		            attrName = inside.Substring(0, eq).Trim().ToLower();
		            attrValue = inside.Substring(eq + 1).Trim().Trim('\'', '"');
		        } else {
		            attrName = inside.Trim().ToLower();
		        }
		    } else {
		        tag = selector.ToLower();
		    }
		
		    foreach (HtmlNode node in Traverse("*")) {
		        if (!string.IsNullOrEmpty(tag) && tag != "*" && node.TagName != tag) continue;
		        if (!string.IsNullOrEmpty(attrName)) {
		            string val;
		            if (!node.Attributes.TryGetValue(attrName, out val)) continue;
		            if (!string.IsNullOrEmpty(attrValue) && val != attrValue) continue;
		        }
		        yield return node;
		    }
		}
		
		private IEnumerable<HtmlNode> Traverse(string tag) {
		    if (tag == "*" || this.TagName == tag) yield return this;
		    foreach (var child in Children)
		        foreach (var c in child.Traverse(tag))
		            yield return c;
		}
	}
}