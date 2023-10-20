using System;
using System.Xml;

namespace ConsoleBrowserObjects {
	public class Textbox : ConsoleBrowserObjects.StdConsoleObject, System.Xml.Serialization.IXmlSerializable {
		private ConsoleColor _DEFAULT_BACKGROUND = ConsoleColor.White;
		private ConsoleColor _DEFAULT_FOREGROUND = ConsoleColor.Black;

		private char _passwordChar = char.MinValue;
		private bool _focus = false;
		internal Textbox() {
			this.InitializeColours();
		}

		public Textbox (string name, Point location, Int32 length) : base(name, location, length) {
			this.InitializeColours();
		}

		public Textbox (string name, Point location, Int32 length, string text) : base(name, location, length, text) {
			this.InitializeColours();
		}

		public Textbox (string name, Point location, Int32 length, string text, ConsoleColor foreground, ConsoleColor background) : base(name, location, length, text, foreground, background) { }

		private void InitializeColours() {
			this.Background = _DEFAULT_BACKGROUND;
			this.Foreground = _DEFAULT_FOREGROUND;
		}
		internal bool Focus {
			get { return _focus; }
			set { _focus = value; }
		}

		internal string NonEventingText {
			get { return base._text; }
			set { base._text = value; }
		}
		public char PasswordChar {
			get { return _passwordChar; }
			set { _passwordChar = value; }
		}
		void System.Xml.Serialization.IXmlSerializable.ReadXml(XmlReader reader) {
			string password = reader.GetAttribute("PasswordChar");

			if (password != null)
				_passwordChar = char.Parse(password);

			base.ReadXml(reader);
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);

			if (_passwordChar != char.MinValue)
				writer.WriteAttributeString("PasswordChar", _passwordChar.ToString());

			base.WriteXml(writer);
			writer.WriteEndElement();
		}
	}
}
