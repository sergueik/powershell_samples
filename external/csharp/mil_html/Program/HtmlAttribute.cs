using System;
using System.Collections;
using System.ComponentModel;

namespace MIL.Html {
	public class HtmlAttribute {
		protected string mName;
		protected string mValue;

		public HtmlAttribute() {
			mName = "Unnamed";
			mValue = "";
		}

		public HtmlAttribute(string name, string value) {
			mName = name;
			mValue = value;
		}

		[
			Category("General"),
			Description("The name of the attribute")
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
			Description("The value of the attribute")
		]
		public string Value {
			get {
				return mValue;
			}
			set {
				mValue = value;
			}
		}

		public override string ToString() {
			if (mValue == null) {
				return mName;
			} else {
				return mName + "=\"" + mValue + "\"";
			}
		}

		[
			Category("Output"),
			Description("The HTML to represent this attribute")
		]
		public string HTML {
			get {
				if (mValue == null) {
					return mName;
				} else {
					return mName + "=\"" + HtmlEncoder.EncodeValue(mValue) + "\"";
				}
			}
		}

		[
			Category("Output"),
			Description("The XHTML to represent this attribute")
		]
		public string XHTML {
			get {
				if (mValue == null) {
					return mName.ToLower();
				} else {
					return mName + "=\"" + HtmlEncoder.EncodeValue(mValue.ToLower()) + "\"";
				}
			}
		}
	}

	public class HtmlAttributeCollection: CollectionBase {
		HtmlElement mElement;

		public HtmlAttributeCollection()
		{
			mElement = null;
		}

		internal HtmlAttributeCollection(HtmlElement element) {
			mElement = element;
		}

		public int Add(HtmlAttribute attribute) {
			return base.List.Add(attribute);
		}

		public HtmlAttribute this[int index] {
			get {
				return (HtmlAttribute)base.List[index];
			}
			set {
				base.List[index] = value;
			}
		}

		public HtmlAttribute FindByName(string name) {
			int index = IndexOf(name);
			if (index == -1) {
				return null;
			} else {
				return this[IndexOf(name)];
			}
		}

		public int IndexOf(string name) {
			for (int index = 0; index < this.List.Count; index++) {
				if (this[index].Name.ToLower().Equals(name.ToLower())) {
					return index;
				}
			}
			return -1;
		}

		public HtmlAttribute this[string name] {
			get {
				return FindByName(name);
			}
		}

	}
}
