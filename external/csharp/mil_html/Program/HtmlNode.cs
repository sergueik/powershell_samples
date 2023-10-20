using System;
using System.Collections;
using System.ComponentModel;

namespace MIL.Html {
	public abstract class HtmlNode {
		protected HtmlElement mParent;

		protected HtmlNode() {
			mParent = null;
		}

		public abstract override string ToString();

		[
		Category("Navigation"),
		Description("The parent node of this one")
		]
		public HtmlElement Parent {
			get {
				return mParent;
			}
		}

		[
		Category("Navigation"),
		Description("The next sibling node")
		]
		public HtmlNode Next {
			get {
				if (Index == -1) {
					return null;
				} else {
					if (Parent.Nodes.Count > Index + 1) {
						return Parent.Nodes[Index + 1];
					} else {
						return null;
					}
				}
			}
		}

		[
		Category("Navigation"),
		Description("The previous sibling node")
		]
		public HtmlNode Previous {
			get {
				if (Index == -1) {
					return null;
				} else {
					if (Index > 0) {
						return Parent.Nodes[Index - 1];
					} else {
						return null;
					}
				}
			}
		}

		[
		Category("Navigation"),
		Description("The first child of this node")
		]
		public HtmlNode FirstChild {
			get {
				if (this is HtmlElement) {
					if (((HtmlElement)this).Nodes.Count == 0) {
						return null;
					} else {
						return ((HtmlElement)this).Nodes[0];
					}
				} else {
					return null;
				}
			}
		}

		[
		Category("Navigation"),
		Description("The last child of this node")
		]
		public HtmlNode LastChild {
			get {
				if (this is HtmlElement) {
					if (((HtmlElement)this).Nodes.Count == 0) {
						return null;
					} else {
						return ((HtmlElement)this).Nodes[((HtmlElement)this).Nodes.Count - 1];
					}
				} else {
					return null;
				}
			}
		}

		[
		Category("Navigation"),
		Description("The zero-based index of this node in the parent's nodes collection")
		]
		public int Index {
			get {
				if (mParent == null) {
					return -1;
				} else {
					return mParent.Nodes.IndexOf(this);
				}
			}
		}

		[
		Category("Navigation"),
		Description("Is this node a root node?")
		]
		public bool IsRoot {
			get {
				return mParent == null;
			}
		}

		[
		Category("Navigation"),
		Description("Is this node a child of another?")
		]
		public bool IsChild {
			get {
				return mParent != null;
			}
		}

		[
		Category("Navigation"),
		Description("Does this node have any children?")
		]
		public bool IsParent {
			get {
				if (this is HtmlElement) {
					return ((HtmlElement)this).Nodes.Count > 0;
				} else {
					return false;
				}
			}
		}

		[
		Category("Relationships")
		]
		public bool IsDescendentOf(HtmlNode node) {
			HtmlNode parent = mParent;
			while (parent != null) {
				if (parent == node) {
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		[
		Category("Relationships")
		]
		public bool IsAncestorOf(HtmlNode node) {
			return node.IsDescendentOf(this);
		}

		[
		Category("Relationships")
		]
		public HtmlNode GetCommonAncestor(HtmlNode node) {
			HtmlNode thisParent = this;
			while (thisParent != null) {
				HtmlNode thatParent = node;
				while (thatParent != null) {
					if (thisParent == thatParent) {
						return thisParent;
					}
					thatParent = thatParent.Parent;
				}
				thisParent = thisParent.Parent;
			}
			return null;
		}

		[
		Category("General")
		]
		public void Remove() {
			if (mParent != null) {
				mParent.Nodes.RemoveAt(this.Index);
			}
		}

		internal void SetParent(HtmlElement parentNode) {
			mParent = parentNode;
		}

		[
		Category("Output"),
		Description("The HTML that represents this node and all the children")
		]
		public abstract string HTML { get; }

		[
		Category("Output"),
		Description("The XHTML that represents this node and all the children")
		]
		public abstract string XHTML { get; }

		[
		Category("General"),
		Description("This is true if this is a text node")
		]
		public bool IsText() {
			return this is HtmlText;
		}

		[
		Category("General"),
		Description("This is true if this is an element node")
		]
		public bool IsElement()
		{
			return this is HtmlElement;
		}
	}

	public class HtmlNodeCollection: CollectionBase {
		private HtmlElement mParent;

		// Public constructor to create an empty collection.
		public HtmlNodeCollection() {
			mParent = null;
		}

		internal HtmlNodeCollection(HtmlElement parent) {
			mParent = parent;
		}

		public int Add(HtmlNode node) {
			if (mParent != null)
				node.SetParent(mParent);
			return base.List.Add(node);
		}

		public int IndexOf(HtmlNode node) {
			return base.List.IndexOf(node);
		}

		public void Insert(int index, HtmlNode node) {
			if (mParent != null)
				node.SetParent(mParent);
			base.InnerList.Insert(index, node);
		}

		public HtmlNode this[int index] {
			get {
				return (HtmlNode)base.InnerList[index];
			}
			set {
				if (mParent != null)
					value.SetParent(mParent);
				base.InnerList[index] = value;
			}
		}

		public HtmlNode this[string name] {
			get {
				HtmlNodeCollection results = FindByName(name, false);
				if (results.Count > 0) {
					return results[0];
				} else {
					return null;
				}
			}
		}

		public HtmlNodeCollection FindByName(string name) {
			return FindByName(name, true);
		}

		public HtmlNodeCollection FindByName(string name, bool searchChildren) {
			var results = new HtmlNodeCollection(null);
			foreach (HtmlNode node in base.List) {
				if (node is HtmlElement) {
					if (((HtmlElement)node).Name.ToLower().Equals(name.ToLower())) {
						results.Add(node);
					}
					if (searchChildren) {
						foreach (HtmlNode matchedChild in ( (HtmlElement)node ).Nodes.FindByName( name , searchChildren )) {
							results.Add(matchedChild);
						}
					}
				}
			}
			return results;
		}

		public HtmlNodeCollection FindByAttributeName(string attributeName) {
			return FindByAttributeName(attributeName, true);
		}

		public HtmlNodeCollection FindByAttributeName(string attributeName, bool searchChildren) {
			var results = new HtmlNodeCollection(null);
			foreach (HtmlNode node in base.List) {
				if (node is HtmlElement) {
					foreach (HtmlAttribute attribute in ((HtmlElement)node).Attributes) {
						if (attribute.Name.ToLower().Equals(attributeName.ToLower())) {
							results.Add(node);
							break;
						}
					}
					if (searchChildren) {
						foreach (HtmlNode matchedChild in ( (HtmlElement)node ).Nodes.FindByAttributeName( attributeName , searchChildren )) {
							results.Add(matchedChild);
						}
					}
				}
			}
			return results;
		}

		public HtmlNodeCollection FindByAttributeNameValue(string attributeName, string attributeValue)
		{
			return FindByAttributeNameValue(attributeName, attributeValue, true);
		}

		public HtmlNodeCollection FindByAttributeNameValue(string attributeName, string attributeValue, bool searchChildren)
		{
			HtmlNodeCollection results = new HtmlNodeCollection(null);
			foreach (HtmlNode node in base.List) {
				if (node is HtmlElement) {
					foreach (HtmlAttribute attribute in ((HtmlElement)node).Attributes) {
						if (attribute.Name.ToLower().Equals(attributeName.ToLower())) {
							if (attribute.Value.ToLower().Equals(attributeValue.ToLower())) {
								results.Add(node);
							}
							break;
						}
					}
					if (searchChildren) {
						foreach (HtmlNode matchedChild in ( (HtmlElement)node ).Nodes.FindByAttributeNameValue( attributeName , attributeValue , searchChildren )) {
							results.Add(matchedChild);
						}
					}
				}
			}
			return results;
		}
	}
}
