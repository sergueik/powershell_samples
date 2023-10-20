using System;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace MIL.Html {
	public class HtmlDocument {
		HtmlNodeCollection mNodes = new HtmlNodeCollection(null);
		private string mXhtmlHeader = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";

		internal HtmlDocument(string html, bool wantSpaces) {
			var parser = new HtmlParser();
			parser.RemoveEmptyElementText = !wantSpaces;
			mNodes = parser.Parse(html);
		}

		[
			Category("General"),
			Description("This is the DOCTYPE for XHTML production")
		]
		public string DocTypeXHTML {
			get {
				return mXhtmlHeader;
			}
			set {
				mXhtmlHeader = value;
			}
		}

		public HtmlNodeCollection Nodes {
			get {
				return mNodes;
			}
		}

		public static HtmlDocument Create(string html) {
			return new HtmlDocument(html, false);
		}

		public static HtmlDocument Create(string html, bool wantSpaces) {
			return new HtmlDocument(html, wantSpaces);
		}


		[
			Category("Output"),
			Description("The HTML version of this document")
		]
		public string HTML {
			get {
				var html = new StringBuilder();
				foreach (HtmlNode node in Nodes) {
					html.Append(node.HTML);
				}
				return html.ToString();
			}
		}

		[
			Category("Output"),
			Description("The XHTML version of this document")
		]
		public string XHTML {
			get {
				StringBuilder html = new StringBuilder();
				if (mXhtmlHeader != null) {
					html.Append(mXhtmlHeader);
					html.Append("\r\n");
				}
				foreach (HtmlNode node in Nodes) {
					html.Append(node.XHTML);
				}
				return html.ToString();
			}
		}
	}
}
