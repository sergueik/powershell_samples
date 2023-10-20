using System;
using System.Text;
using System.ComponentModel;

namespace MIL.Html {

	public class HtmlElement: HtmlNode {
		protected string mName;
		protected HtmlNodeCollection mNodes;
		protected HtmlAttributeCollection mAttributes;
		protected bool mIsTerminated;
		protected bool mIsExplicitlyTerminated;

		public HtmlElement(string name) {
			mNodes = new HtmlNodeCollection(this);
			mAttributes = new HtmlAttributeCollection(this);
			mName = name;
			mIsTerminated = false;
		}

		[
		Category("General"),
		Description("The name of the tag/element")
		]
		public string Name {
			get {
				return mName;
			}
			set {
				mName = value;
			}
		}

		[
		Category("General"),
		Description("The set of child nodes")
		]
		public HtmlNodeCollection Nodes {
			get {
				if (IsText()) {
					throw new InvalidOperationException("An HtmlText node does not have child nodes");
				}
				return mNodes;
			}
		}

		[
		Category("General"),
		Description("The set of attributes associated with this element")
		]
		public HtmlAttributeCollection Attributes {
			get {
				return mAttributes;
			}
		}

		internal bool IsTerminated {
			get {
				if (Nodes.Count > 0) {
					return false;
				} else {
					return mIsTerminated | mIsExplicitlyTerminated;
				}
			}
			set {
				mIsTerminated = value;
			}
		}

		internal bool IsExplicitlyTerminated {
			get {
				return mIsExplicitlyTerminated;
			}
			set {
				mIsExplicitlyTerminated = value;
			}
		}

		internal bool NoEscaping {
			get {
				return "script".Equals(Name.ToLower()) || "style".Equals(Name.ToLower());
			}
		}

		public override string ToString() {
			string value = "<" + mName;
			foreach (HtmlAttribute attribute in Attributes) {
				value += " " + attribute.ToString();
			}
			value += ">";
			return value;
		}

		[
		Category("General"),
		Description("A concatination of all the text associated with this element")
		]
		public string Text {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				foreach (HtmlNode node in Nodes) {
					if (node is HtmlText) {
						stringBuilder.Append(((HtmlText)node).Text);
					}
				}
				return stringBuilder.ToString();
			}
		}

		[
		Category("Output")
		]
		public override string HTML {
			get {
				var html = new StringBuilder();
				html.Append("<" + mName);
				foreach (HtmlAttribute attribute in Attributes) {
					html.Append(" " + attribute.HTML);
				}
				if (Nodes.Count > 0) {
					html.Append(">");
					foreach (HtmlNode node in Nodes) {
						html.Append(node.HTML);
					}
					html.Append("</" + mName + ">");
				} else {
					if (IsExplicitlyTerminated) {
						html.Append("></" + mName + ">");
					} else if (IsTerminated) {
						html.Append("/>");
					} else {
						html.Append(">");
					}
				}
				return html.ToString();
			}
		}

		[
		Category("Output")
		]
		public override string XHTML {
			get {
				if ("html".Equals(mName) && this.Attributes["xmlns"] == null) {
					Attributes.Add(new HtmlAttribute("xmlns", "http://www.w3.org/1999/xhtml"));
				}
				var html = new StringBuilder();
				html.Append("<" + mName.ToLower());
				foreach (HtmlAttribute attribute in Attributes) {
					html.Append(" " + attribute.XHTML);
				}
				if (IsTerminated) {
					html.Append("/>");
				} else {
					if (Nodes.Count > 0) {
						html.Append(">");
						foreach (HtmlNode node in Nodes) {
							html.Append(node.XHTML);
						}
						html.Append("</" + mName.ToLower() + ">");
					} else {
						html.Append("/>");
					}
				}
				return html.ToString();
			}
		}
	}
}
