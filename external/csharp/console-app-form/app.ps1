Add-Type -TypeDefinition @'
using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleBrowserObjects {
	public class ConsoleForm : IXmlSerializable, IDisposable {
		public delegate void onKeyPress(ConsoleForm sender, KeyPressEventArgs e);

		public delegate void onFormComplete(ConsoleForm sender, FormCompleteEventArgs e);

		public delegate void onFormCancelled(ConsoleForm sender, System.EventArgs e);

		private onKeyPress _keyPressEvent = null;

		public event onKeyPress KeyPressed {
			add {
				if (_keyPressEvent == null)
					_keyPressEvent = value;
				else
					throw new InvalidOperationException("Can only wire 1 handler to this event.");
			}
			remove {
				if (_keyPressEvent == value)
					_keyPressEvent = null;
				else
					throw new InvalidOperationException("You can't unhook an unwired event.");
			}
		}

		private onFormComplete _formCompleteEvent = null;

		public event onFormComplete FormComplete {
			add {
				if (_formCompleteEvent == null)
					_formCompleteEvent = value;
				else
					throw new InvalidOperationException("Can only wire 1 handler to this event.");
			}
			remove {
				if (_formCompleteEvent == value)
					_formCompleteEvent = null;
				else
					throw new InvalidOperationException("You can't unhook an unwired event.");
			}
		}

		public event onFormCancelled FormCancelled;

		private Labels _labels = new Labels();
		private Textboxes _textboxes = new Textboxes();
		private Lines _lines = new Lines();
		private string _name = string.Empty;
		private Int32 _currentField = 0;
		private Int32 _width = 80;
		private Int32 _height = 40;

		// Textbox with focus
		private Textbox _field = null;
		private Thread _keyThread = null;
		private ThreadStart _keyThreadStart = null;

		private ConsoleForm() {
			// Initialize the keypress thread variables.
			_keyThreadStart = new ThreadStart(LoopForKeypress);
			_keyThread = new Thread(_keyThreadStart);
		}
		public ConsoleForm(Int32 width, Int32 height) : this() {
			_width = width;
			_height = height;
		}
		~ConsoleForm() { }

		public static ConsoleForm GetFormInstance(string path) {
			var form = new ConsoleForm();
			var ser = new XmlSerializer(form.GetType());

			if (path.IndexOf(".\\") == 0)
				path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + path.Substring(1);

			using (var sr = new StreamReader(path)) {
				form = (ConsoleForm)ser.Deserialize(sr);

				sr.Close();
			}

			return form;
		}

		public static ConsoleForm GetFormInstance(string path, onFormComplete formComplete, onFormCancelled formCancelled) {
			// Call the other static factory method to get a new console form.
			ConsoleForm form = ConsoleForm.GetFormInstance(path);

			if (formComplete != null)
				form.FormComplete += formComplete;

			if (formCancelled != null)
				form.FormCancelled += formCancelled;

			return form;
		}
 
		private void Refresh(StdConsoleObject sco) {
			// Mark the starting position and colour of the console.
			Int32 x = Console.CursorLeft;
			Int32 y = Console.CursorTop;

			ConsoleColor fore = Console.ForegroundColor;
			ConsoleColor back = Console.BackgroundColor;

			// Make sure the data being written to the screen is either
			// truncated if too long, or padded if too short, to make the
			// field being shown appear correct.
			string text = string.Empty;

			// Make sure to refresh with password masking characters if this is
			// a password field.
			if (sco is Textbox && ((Textbox)sco).PasswordChar != char.MinValue)
				text = new string(((Textbox)sco).PasswordChar, sco.Text.Length);
			else
				text = sco.Text;

			if (text.Length > sco.Length)
				text = text.Substring(0, sco.Length);

			if (text.Length < sco.Length)
				text = text.PadRight(sco.Length, ' ');

			// Actually write the text
			Console.SetCursorPosition(sco.Location.X, sco.Location.Y);
			Console.BackgroundColor = sco.Background;
			Console.ForegroundColor = sco.Foreground;
			Console.Write(text);

			// Reset the cursor and colour information.
			Console.ForegroundColor = fore;
			Console.BackgroundColor = back;
			Console.SetCursorPosition(x, y);

			// If the field being updated is the "current" field (i.e. the one with the 
			// cursor in it), reposition the cursor to accomodate existing text.
			if (sco is Textbox)
			if (((Textbox)sco) == _field)
				Console.SetCursorPosition(_field.Location.X + _field.Text.Length, 
					_field.Location.Y);
		}

		public void SetFocus(Textbox field) {
			for (Int32 i = 0; i < _textboxes.Count; i++)
				if (_textboxes[i] == field) {
					_field = Textboxes[i];
					_currentField = i;

					Console.ForegroundColor = _field.Foreground;
					Console.BackgroundColor = _field.Background;
					Console.SetCursorPosition(_field.Location.X + _field.Text.Length, _field.Location.Y);

					return;
				}

			throw new InvalidOperationException(field.Name + " not found.");
		}

		public void Render() {
			this.Render(true);
		}

		public void Render(bool clear) {
			Console.ResetColor();
 
			if (clear)
				Console.Clear();

			Console.Title = _name;

			// Resize the window and the buffer to the form's size.
			if (Console.BufferHeight != _height || Console.BufferWidth != _width) {
				Console.SetWindowSize(_width, _height);
				Console.SetBufferSize(_width, _height);
			}

			if (Console.WindowHeight != _height || Console.WindowWidth != _width) {
				Console.SetBufferSize(_width, _height);
				Console.SetWindowSize(_width, _height);
			}

			// Draw the lines first.
			foreach (Line line in _lines) {
				Console.BackgroundColor = line.Colour;

				if (line.Orientation == Line.LineOrientation.Horizontal) {
					// Instructions for drawing a horizontal line.
					Console.SetCursorPosition(line.Location.X, line.Location.Y);
					Console.Write(new string(' ', line.Length));
				} else {
					// Instructions for drawing a vertical line.
					Int32 x = line.Location.X;

					for (Int32 i = line.Location.Y; i < line.Location.Y + line.Length; i++) {
						Console.SetCursorPosition(x, i);
						Console.Write(" ");
					}
				}
			}

			// Draw the labels next.
			foreach (Label label in _labels)
				Refresh(label);

			// Now draw the textboxes.
			foreach (Textbox text in _textboxes)
				Refresh(text);

			// If any textboxes are defined for the form, pick the first one and position
			// the cursor accordingly.
			if (_textboxes.Count > 0) {
				_field = _textboxes[0];
				_textboxes.FocusField = _field;
				Console.SetCursorPosition(_field.Location.X + _field.Text.Length, _field.Location.Y);
				Console.CursorVisible = true;
			} else
 // Otherwise, hide the cursor.
 Console.CursorVisible = false;

			_labels.Rendered();
			_textboxes.Rendered();

			if (_keyThread.Name == null) {
				// Start the thread that listens for keypresses.
				_keyThread.Name = "Keypress loop for " + _name;
				_keyThread.Start();
			}
		}

		private void LoopForKeypress() {
			// Loop for keypresses. Since we're doing all the work of processing, we have
			// to trap special keypresses and respond appropriately
			while (true) {
				// Blocks on the next function call.
				ConsoleKeyInfo cki = Console.ReadKey(true);

				ConsoleKey nKey = cki.Key;

				// A key's been pressed. Figure out what to do.
				// All actions will be against the current field, stored in _field.
				char cChar = cki.KeyChar;

				if (cChar != 0) { // Guard against unprintable chars.
					KeyPressEventArgs kpea = new KeyPressEventArgs(_field, cChar);

					if (_keyPressEvent != null)
						_keyPressEvent(this, kpea);

					if (!kpea.Cancel) { // Process the keystroke. It wasn't cancelled.
						switch (nKey) {
							case ConsoleKey.Backspace: // Backspace pressed
 // Is there a character to backspace over?
								if (_field.Text.Length > 0) {
									_field.Text = _field.Text.Substring(0, _field.Text.Length - 1);
									Refresh(_field);
								}

								break;

							case ConsoleKey.Tab: // Tab -> Move to the next field.
								if (cki.Modifiers == ConsoleModifiers.Shift) { 
									// Go backwards.
									_currentField--;

									// If we're at the first field, move to the last.
									if (_currentField == -1)
										_currentField = _textboxes.Count - 1;
								} else {
									// Go forwards
									_currentField++;

									// If we're in the last field already, move back to the first.
									if (_currentField == _textboxes.Count)
										_currentField = 0;
								}

 // Set the current field to the next one in the collection.
								_field = _textboxes[_currentField];
								_textboxes.FocusField = _field;

 // Move the cursor to the location of the next field, accomodating
 // any text that may already be there..
								Console.SetCursorPosition(_field.Location.X + _field.Text.Length, _field.Location.Y);
								break;

							case ConsoleKey.Enter: // Enter -> Fire the complete event if it's wired.
								if (_formCompleteEvent != null) {
									FormCompleteEventArgs fcea = new FormCompleteEventArgs();

									_formCompleteEvent(this, fcea);

									// The listener of this event will set the Cancel field if they
									// want to re-use the form. If not cancelled, the form will
									// be destroyed.
									if (!fcea.Cancel) {
										// Get rid of this form. A new one will be displayed.

										// Unusual to call Dispose() on oneself, but it saves a lot of
										// code in the clients if this is the default behaviour, rather
										// than forcing every single event to call Dispose() on the
										// passed-in sender parameter.
										((IDisposable)this).Dispose();
										return;
									} // else the current form will be reused. Go back for more keys.
								}

								break;

							case ConsoleKey.Escape: // Esc -> Fire the cancelled event if it's wired.
								if (this.FormCancelled != null) {
									this.FormCancelled(this, System.EventArgs.Empty);

									((IDisposable)this).Dispose();
									return;
								}

								break;

							default:
								if (_field != null) {
									// no keystrokes accepted oncd field is full 
									if (_field.Text.Length < _field.Length) {
										_field.NonEventingText += cChar;
										Console.ForegroundColor = _field.Foreground;
										Console.BackgroundColor = _field.Background;
										// password field uses the password character
										// regular text field shows the actual character.
										if (_field.PasswordChar != char.MinValue) Console.Write(_field.PasswordChar);
										else										
										Console.Write(cChar);
									}
								}
								break;
						}
					}
				}
			}
		}

		public Int32 Width { get { return _width; } }

		public Int32 Height { get { return _height; } }

		public string Name {
			get { return _name; }
			set { _name = value; }
		}

		public Lines Lines {
			get { return _lines; }
		}

		public Labels Labels {
			get { return _labels; }
		}

		public Textboxes Textboxes {
			get { return _textboxes; }
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteAttributeString(string.Empty, "Name", string.Empty, _name);

			((IXmlSerializable)_lines).WriteXml(writer);
			((IXmlSerializable)_labels).WriteXml(writer);
			((IXmlSerializable)_textboxes).WriteXml(writer);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			// Read the name of the form.
			_name = reader.GetAttribute("Name");

			// Get the width and height of the form, if they're specified.
			if (reader.GetAttribute("Width") != null)
				_width = Int32.Parse(reader.GetAttribute("Width"));

			if (reader.GetAttribute("Height") != null)
				_height = Int32.Parse(reader.GetAttribute("Height"));

			// Move to the node after the <ConsoleForm> node.
			reader.Read();
 
			// Expect to see a node of Line objects.
			if (reader.Name == "Lines")
				((IXmlSerializable)_lines).ReadXml(reader);
			else
				throw new InvalidOperationException("<Lines> element missing from form definition.");

			// Now expect to see a node containing the Label objects.
			if (reader.Name == "Labels")
				((IXmlSerializable)_labels).ReadXml(reader);
			else
				throw new InvalidOperationException("<Labels> element missing from form definition.");

			// Finally, we expect to see the node containing the Textbox objects.
			if (reader.Name == "Textboxes")
				((IXmlSerializable)_textboxes).ReadXml(reader);
			else
				throw new InvalidOperationException("<Textboxes> element missing from form definition.");
		}
		void IDisposable.Dispose() {
			((IDisposable)_labels).Dispose();
			((IDisposable)_textboxes).Dispose();
			((IDisposable)_lines).Dispose();

			// Unwire any listening events.
			_keyPressEvent = null;
			_formCompleteEvent = null;
			FormCancelled = null;

			// Terminate the keypress loop.
			_keyThread.Abort();
			_keyThread = null;

			GC.SuppressFinalize(this);
		}
	}

	public class FormCompleteEventArgs : System.EventArgs {
		private bool _cancel = false;

		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}
	}

	public class KeyPressEventArgs : System.EventArgs {
		private bool _cancel = false;
		private char _char = char.MinValue;
		private Textbox _field = null;
		public KeyPressEventArgs(Textbox field, char c) {
			_field = field;
			_char = c;
		}
		public Textbox Textbox { get { return _field; } }
		public char Char { get { return _char; } }

		public bool Cancel {
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
	
	public class Label : ConsoleBrowserObjects.StdConsoleObject, IXmlSerializable {
		private ConsoleColor _DEFAULT_BACKGROUND = ConsoleColor.Black;
		private ConsoleColor _DEFAULT_FOREGROUND = ConsoleColor.White;
		internal Label() { this.InitializeColours(); }

		public Label (string name, Point location, Int32 length) : base(name, location, length) {
			this.InitializeColours();
		}

		public Label (string name, Point location, Int32 length, string text) : base(name, location, length, text) {
			this.InitializeColours();
		}

		public Label (string name, Point location, Int32 length, string text, ConsoleColor foreground, ConsoleColor background) : base(name, location, length, text, foreground, background) {
		}
		private	void InitializeColours() {
			this.Foreground = _DEFAULT_FOREGROUND;
			this.Background = _DEFAULT_BACKGROUND;
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);
			base.WriteXml(writer);
			writer.WriteEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			// Nothing to do but ship the work off to the base class.
			base.ReadXml(reader);
		}
	}

	public class Labels : IEnumerable, IXmlSerializable, IDisposable {
		private List<Label> _labels = new List<Label>();
		// The container of the Label objects.
		public Labels() { }
		public Int32 Count { get { return _labels.Count; } }
		internal void Rendered() {
			foreach (StdConsoleObject sco in _labels)
				sco.Rendered();
		}
		public void Add(Label l) {
			_labels.Add(l);
		}
		public Label this[string name] {
			get {
				foreach (Label cLabel in _labels)
					if (cLabel.Name == name)
						return cLabel;

				return null;
			}
		}

		public Label this[Int32 index] {
			get {
				return _labels[index];
			}
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);

			// Persist the state of the labels in the collection.
			foreach (IXmlSerializable l in _labels)
				l.WriteXml(writer);

			writer.WriteEndElement();
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			// TODO: Add Labels.GetSchema implementation
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			// Advance until the first Label node is recognized.
			if (!reader.IsEmptyElement) {
				while (reader.Read()) {
					if (reader.NodeType == System.Xml.XmlNodeType.EndElement) {
						reader.Read();
						break;
					}

					// Found a Label node. Deserialize it into a new Label object.
					ConsoleBrowserObjects.Label label = new Label();

					((IXmlSerializable)label).ReadXml(reader);
					_labels.Add(label);

					// Move to the next node.
					reader.Read();
				}
			} else // The reader is empty. Advance to the next node in anticipation
 // of finding a Textboxes node.
 reader.Read();
		}
		public void Dispose() {
			foreach (IDisposable label in _labels)
				label.Dispose();

			_labels.Clear();

			GC.SuppressFinalize(this);
		}
		public System.Collections.IEnumerator GetEnumerator() {
			return new LabelEnumerator(_labels);
		}
		private class LabelEnumerator : IEnumerator {

 	
			private List<Label> _labels = null;
			private Int32 _pointer = -1;
			public LabelEnumerator(List<Label> labels)
			{
				_labels = labels;
			}
			public object Current {
				get { return _labels[_pointer]; }
			}

			public bool MoveNext() {
				return ++_pointer != _labels.Count;
			}

			public void Reset() {
				_pointer = -1;
			}
		}

	}
	
	public class Line : System.Xml.Serialization.IXmlSerializable, System.IDisposable {
 
		public enum LineOrientation {
			Vertical = 0,
			Horizontal = 1
		}
		private LineOrientation _orientation = Line.LineOrientation.Horizontal;
		private ConsoleColor _colour;
		private Point _location = new Point();
		private Int32 _length = 0;
		public Line() { }

		public Line (LineOrientation orientation, ConsoleColor colour, Point location, Int32 length) : this() {
			_orientation = orientation;
			_colour = colour;
			_location = location;
			_length = length;
		}
		public LineOrientation Orientation {
			get { return _orientation; }
			set { _orientation = value; }
		}

		public ConsoleColor Colour {
			get { return _colour; }
			set { _colour = value; }
		}

		public Point Location {
			get { return _location; }
			set { _location = value; }
		}

		public Int32 Length {
			get { return _length; }
			set { _length = value; }
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);
			writer.WriteAttributeString(string.Empty, "Orientation", string.Empty, _orientation.ToString());
			writer.WriteAttributeString(string.Empty, "Length", string.Empty, _length.ToString());
			writer.WriteAttributeString(string.Empty, "Colour", string.Empty, _colour.ToString());

			((IXmlSerializable)_location).WriteXml(writer);

			writer.WriteEndElement();
		}
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			// TODO: Add Line.GetSchema implementation
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			_orientation = (LineOrientation)Enum.Parse(_orientation.GetType(), 
				reader.GetAttribute("Orientation"));
			_length = Int32.Parse(reader.GetAttribute("Length"));

			string colour = reader.GetAttribute("Colour");
			if (colour != null && colour.Length > 0)
				_colour = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colour);

			reader.Read();

			if (reader.Name == "Location")
				((IXmlSerializable)_location).ReadXml(reader);
			else
				throw new InvalidOperationException("<Location> node missing from <Line> node.");
		}

		void IDisposable.Dispose() {
			((IDisposable)_location).Dispose();
			GC.SuppressFinalize(this);
		}
	}
	
	public class Lines : IEnumerable, IXmlSerializable, IDisposable {
		private List<Line> _lines = new List<Line>();
		public Lines() { }
		public Int32 Count { get { return _lines.Count; } }
		public void Add(Line line) {
			_lines.Add(line);
		}
		public Line this[Int32 index] {
			get { return _lines[index]; }
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);

			foreach (IXmlSerializable line in _lines)
				line.WriteXml(writer);

			writer.WriteEndElement();
		}

		// TODO: Add Lines.GetSchema implementation
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			if (!reader.IsEmptyElement) {
				while (reader.Read()) {
					if (reader.NodeType == System.Xml.XmlNodeType.EndElement) {
						reader.Read();
						break;
					}

					ConsoleBrowserObjects.Line line = new Line();

					((IXmlSerializable)line).ReadXml(reader);
					_lines.Add(line);

					reader.Read();
				}
			} else
				reader.Read();
		}

		void IDisposable.Dispose() {
			foreach (IDisposable line in _lines)
				line.Dispose();

			_lines.Clear();

			GC.SuppressFinalize(this);
		}

		public IEnumerator GetEnumerator() {
			return new LineEnumerator(_lines);
		}
		private class LineEnumerator : System.Collections.IEnumerator {
			private List<Line> _lines = null;
			private Int32 _pointer = -1;

			public LineEnumerator(List<Line> lines) {
				_lines = lines;
			}

			public object Current {
				get { return _lines[_pointer]; }
			}

			public bool MoveNext() {
				return ++_pointer != _lines.Count;
			}

			public void Reset() {
				_pointer = -1;
			}
		}
	}	
	public class Point : IXmlSerializable, IDisposable {
		private Int32 _x = 0;
		private Int32 _y = 0;
		public Point() { }

		public Point(Int32 x, Int32 y) {
			_x = x;
			_y = y;
		}
        
		public Int32 X {
			get { return _x; }
			set { _x = value; }
		}

		public Int32 Y {
			get { return _y; }
			set { _y = value; }
		}
		public override string ToString() {
			return "X: " + _x.ToString() + " Y: " + _y.ToString();
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement("Location");
			writer.WriteAttributeString(string.Empty, "X", string.Empty, X.ToString());
			writer.WriteAttributeString(string.Empty, "Y", string.Empty, Y.ToString());
			writer.WriteEndElement();
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			string x = reader.GetAttribute("X");
			string y = reader.GetAttribute("Y");

			if (x.Length > 0)
				_x = Int32.Parse(x);

			if (y.Length > 0)
				_y = Int32.Parse(y);
		}
		void IDisposable.Dispose() {
			GC.SuppressFinalize(this);
		}
	}
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
	public class Textboxes : IEnumerable,  IXmlSerializable, IDisposable {
		private List<Textbox> _textboxes = new List<Textbox>();
		private Textbox _focusField = null;

		public Textboxes() { }
		public Int32 Count { get { return _textboxes.Count; } }
		internal void Rendered() {
			foreach (StdConsoleObject sco in _textboxes)
				sco.Rendered();
		}
		internal Textbox FocusField {
			get { return _focusField; }
			set {
				if (_focusField != value) {
					_focusField = value;
					foreach (Textbox t in _textboxes)
						t.Focus = (t == _focusField);
				} 
			}
		}
		public void Add(Textbox t) {
			_textboxes.Add(t);
		}

		public void ClearValues() {
			foreach (Textbox t in _textboxes)
				t.Text = string.Empty;
		}

		public Textbox this[string name] {
			get {
				foreach (Textbox cTextbox in _textboxes)
					if (cTextbox.Name == name)
						return cTextbox;

				return null;
			}
		}

		public Textbox this[Int32 index] {
			get {
				return _textboxes[index];
			}
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement(this.GetType().Name);

			foreach (IXmlSerializable tb in _textboxes)
				tb.WriteXml(writer);

			writer.WriteEndElement();
		}

		// TODO: Add Textboxes.GetSchema implementation
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema() {
			return null;
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
			if (!reader.IsEmptyElement) {
				while (reader.Read()) {
					if (reader.NodeType == System.Xml.XmlNodeType.EndElement) {
						reader.Read();
						break;
					}

					var textbox = new Textbox();

					((IXmlSerializable)textbox).ReadXml(reader);
					_textboxes.Add(textbox);

					reader.Read();
				}
			} else
				reader.Read();
		}
		void IDisposable.Dispose() {
			foreach (IDisposable textbox in _textboxes)
				textbox.Dispose();

			_textboxes.Clear();

			GC.SuppressFinalize(this);
		}
		public IEnumerator GetEnumerator() {
			return new TextboxEnumerator(_textboxes);
		}
		private class TextboxEnumerator : System.Collections.IEnumerator {
			private List<Textbox> _textboxes = null;
			private Int32 _pointer = -1;
			public TextboxEnumerator(List<Textbox> textboxes) {
				_textboxes = textboxes;
			}
			public object Current {
				get { return _textboxes[_pointer]; }
			}

			public bool MoveNext() {
				return ++_pointer != _textboxes.Count;
			}

			public void Reset() {
				_pointer = -1;
			}
		}
	}
	public abstract class StdConsoleObject : System.Xml.Serialization.IXmlSerializable, IDisposable {
		private ConsoleColor _foreground;
		private ConsoleColor _background;

		private Point _location = new Point();

		private string _name = string.Empty;
		private Int32 _length = 0;
		protected bool _rendered = false;
		protected string _text = string.Empty;
		internal StdConsoleObject() { }

		public StdConsoleObject (string name, Point location, Int32 length) {
			_name = name;
			_location = location;
			_length = length;
		}

		public StdConsoleObject (string name, Point location, Int32 length, string text) : this(name, location, length) {
			_text = text;
		}

		public StdConsoleObject (string name, Point location, Int32 length, string text, ConsoleColor foreground, ConsoleColor background) : this(name, location, length, text) {
			_foreground = foreground;
			_background = background;
		}
		public ConsoleColor Foreground {
			get { return _foreground; }
			set { _foreground = value; }
		}
		public ConsoleColor Background {
			get { return _background; }
			set { _background = value; }
		}
 
		public Point Location {
			get { return _location; }
		}
 
		public string Name {
			get { return _name; }
		}
 
		public string Text {
			get { return _text; }
			set {
				if (_text != value) {
					_text = value;

					if (_rendered) {
						// Mark the starting position and colour of the console.
						Int32 x = Console.CursorLeft;
						Int32 y = Console.CursorTop;

						ConsoleColor fore = Console.ForegroundColor;
						ConsoleColor back = Console.BackgroundColor;

						// Make sure the data being written to the screen is either
						// truncated if too long, or padded if too short, to make the
						// field being shown appear correct.
						string text = _text;
						if (text.Length > _length)
							text = text.Substring(0, _length);

						if (text.Length < _length)
							text = text.PadRight(_length, ' ');

						// Actually write the text
						Console.SetCursorPosition(_location.X, _location.Y);
						Console.BackgroundColor = _background;
						Console.ForegroundColor = _foreground;
						Console.Write(text);

						// Reset the cursor and colour information.
						Console.ForegroundColor = fore;
						Console.BackgroundColor = back;
						Console.SetCursorPosition(x, y);

						if (this is Textbox)
						if (((Textbox)this).Focus)
							Console.SetCursorPosition(_location.X + _text.Length, _location.Y);
					}
				}
			}
		}
 
		public Int32 Length {
			get { return _length; }
		}
		internal void Rendered() {
			_rendered = true;
		}
		public void WriteXml(System.Xml.XmlWriter writer) {
			writer.WriteAttributeString(string.Empty, "Name", string.Empty, _name);
			writer.WriteAttributeString(string.Empty, "Text", string.Empty, _text);
			writer.WriteAttributeString(string.Empty, "Length", string.Empty, _length.ToString());
			writer.WriteAttributeString(string.Empty, "ForeColour", string.Empty, _foreground.ToString());
			writer.WriteAttributeString(string.Empty, "BackColour", string.Empty, _background.ToString());

			((IXmlSerializable)_location).WriteXml(writer);
		}

		public System.Xml.Schema.XmlSchema GetSchema() {
			// TODO: Add StdConsoleObject.GetSchema implementation
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader) {
			_name = reader.GetAttribute("Name");
			_text = (reader.GetAttribute("Text") == null ? string.Empty : reader.GetAttribute("Text"));
 
			string length = reader.GetAttribute("Length");
			if (length != null && length.Length > 0)
				_length = Int32.Parse(length);
			else
				_length = _text.Length;

			string foreground = reader.GetAttribute("ForeColour");
			if (foreground != null && foreground.Length > 0)
				_foreground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), foreground);

			string background = reader.GetAttribute("BackColour");
			if (background != null && background.Length > 0)
				_background = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), background);

			reader.Read(); 

			if (reader.Name == "Location")
				((IXmlSerializable)_location).ReadXml(reader);
			else
				throw new InvalidOperationException("<Location> node missing from " + _name + " node.");
		}
		public void Dispose() {
			((IDisposable)_location).Dispose();

			GC.SuppressFinalize(this);
		}
	}
	
}

'@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.XML.dll'

[ConsoleBrowserObjects.ConsoleForm] $f = [ConsoleBrowserObjects.ConsoleForm]::GetFormInstance('Demo\Forms\Login.xml', {
  param (
    [ConsoleBrowserObjects.ConsoleForm] $sender, 
    [ConsoleBrowserObjects.FormCompleteEventArgs] $e
  )

  if (($sender.Textboxes["txtLoginID"].Text -eq "test" )-and
    (sender.Textboxes["txtPassword"].Text -eq "test")) {
    # User validated	
    return
  } else {
    # Account not found.
    $sender.Labels["lblError"].Text = "Account not found."
    $sender.Textboxes["txtLoginID"].Text = ''
    $sender.Textboxes["txtPassword"].Text = ''

    $sender.SetFocus($sender.Textboxes["txtLoginID"])

    $e.Cancel = true; 
    # Keep the form visible
}
}, {
  param (
    [ConsoleBrowserObjects.ConsoleForm] $sender, 
    [System.EventArgs] $e
  )

})
# TODO: handle ConsoleForm.onKeyPress delegate somehow
$f.add_KeyPressed({
  param (
    [ConsoleBrowserObjects.ConsoleForm] $sender, 
    [ConsoleBrowserObjects.KeyPressEventArgs] $e
  )
  # If an error was displayed, clear it on this keypress.
  if ($sender.Labels["lblError"].Text -ne $null){
    $sender.Labels["lblError"].Text = $null
  }

})
$f.Render()
