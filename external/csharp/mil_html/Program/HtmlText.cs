using System;
using System.ComponentModel;

namespace MIL.Html {
	public class HtmlText: HtmlNode {
		protected string mText;

		public HtmlText(string text) {
			mText = text;
		}

		[
		Category("General"),
		Description("The text located in this text node")
		]
		public string Text {
			get {
				return mText;
			}
			set {
				mText = value;
			}
		}

		public override string ToString() {
			return Text;
		}

		internal bool NoEscaping {
			get {
				if (mParent == null) {
					return false;
				} else {
					return ((HtmlElement)mParent).NoEscaping;
				}
			}
		}

		public override string HTML {
			get {
				if (NoEscaping) {
					return Text;
				} else {
					return HtmlEncoder.EncodeValue(Text);
				}
			}
		}

		public override string XHTML {
			get {
				return HtmlEncoder.EncodeValue(Text);
			}
		}
	}
}
