using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Program {
	public class NewConsoleForm : IDisposable {
		public delegate void onKeyPress(NewConsoleForm sender, KeyPressEventArgs e);

		public delegate void onFormComplete(NewConsoleForm sender, FormCompleteEventArgs e);

		public delegate void onFormCancelled(NewConsoleForm sender, System.EventArgs e);

		private onKeyPress keyPressEvent = null;

		public event onKeyPress KeyPressed {
			add {
				if (keyPressEvent == null)
					keyPressEvent = value;
				else
					throw new InvalidOperationException("Can only wire 1 handler to this event.");
			}
			remove {
				if (keyPressEvent == value)
					keyPressEvent = null;
				else
					throw new InvalidOperationException("You can't unhook an unwired event.");
			}
		}

		private onFormComplete formCompleteEvent = null;

		public event onFormComplete FormComplete {
			add {
				if (formCompleteEvent == null)
					formCompleteEvent = value;
				else
					throw new InvalidOperationException("Can only wire 1 handler to this event.");
			}
			remove {
				if (formCompleteEvent == value)
					formCompleteEvent = null;
				else
					throw new InvalidOperationException("You can't unhook an unwired event.");
			}
		}

		public event onFormCancelled FormCancelled;

		private Labels labels = new Labels();
		private Textboxes textboxes = new Textboxes();
		private Lines lines = new Lines();
		private string name = string.Empty;
		private Int32 currentField = 0;
		private Int32 width = 80;
		private Int32 height = 40;

		// Textbox with focus
		private Textbox field = null;
		private Thread keyThread = null;
		private ThreadStart keyThreadStart = null;
		private bool debug;
		public Boolean Debug { get { return debug; } set { debug = value; } }

		private NewConsoleForm() {
			// Initialize the keypress thread variables.
			keyThreadStart = new ThreadStart(LoopForKeypress);
			keyThread = new Thread(keyThreadStart);
		}
		public NewConsoleForm(Int32 width, Int32 height) : this() {
			this.width = width;
			this.height = height;
		}
		~NewConsoleForm() { }

		public static string getFileContents(string path) {

			string contents = null;
			if (path.IndexOf(@".\\") == 0) {
				path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + path.Substring(1);
			}
			using (var streamReader = new StreamReader(path)) {
				contents = streamReader.ReadToEnd();
				// Console.WriteLine(contents);
				
				streamReader.Close();
			}
			return contents;
		}	

		public static NewConsoleForm GetFormInstance(string path) {
			var json = getFileContents(path);
			NewConsoleForm form = JsonConvert.DeserializeObject<NewConsoleForm>(json);
			return form;
		}

		public static NewConsoleForm GetFormInstance(string path, onFormComplete formComplete, onFormCancelled formCancelled) {
			// Call the other static factory method to get a new console form.
			var form = NewConsoleForm.GetFormInstance(path);

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
			if (((Textbox)sco) == field)
				Console.SetCursorPosition(field.Location.X + field.Text.Length, 
					field.Location.Y);
		}

		public void SetFocus(Textbox field) {
			for (Int32 i = 0; i < textboxes.Count; i++)
				if (textboxes[i] == field) {
					this.field = Textboxes[i];
					currentField = i;

					Console.ForegroundColor = this.field.Foreground;
					Console.BackgroundColor = this.field.Background;
					Console.SetCursorPosition(this.field.Location.X + this.field.Text.Length, this.field.Location.Y);

					return;
				}

			throw new InvalidOperationException(field.Name + " not found.");
		}

		public Int32 Width { get { return width; } set { width = value; } }

		public Int32 Height { get { return height; } set { height = value; } }

		public string Name { get { return name; } set { name = value; } }

		public Lines Lines { get { return lines; } set { lines = value; } }

		public Labels Labels { get { return labels; } set { labels = value; } }

		public Textboxes Textboxes { get { return textboxes; } set { textboxes = value; } }

		public void Render() {
			this.Render(true);
		}

		public void Render(bool clear) {
			Console.ResetColor();
 
			if (clear)
				Console.Clear();

			Console.Title = name;

			// Resize the window and the buffer to the form's size.
			if (Console.BufferHeight != height || Console.BufferWidth != width) {
				Console.SetWindowSize(width, height);
				Console.SetBufferSize(width, height);
			}

			if (Console.WindowHeight != height || Console.WindowWidth != width) {
				Console.SetBufferSize(width, height);
				Console.SetWindowSize(width, height);
			}

			// Draw the lines first.
			foreach (Line line in lines.Details) {
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
			foreach (Label label in labels.Details)
				Refresh(label);

			// Now draw the textboxes.
			foreach (Textbox text in textboxes.Details)
				Refresh(text);

			// If any textboxes are defined for the form, pick the first one and position
			// the cursor accordingly.
			if (textboxes.Details.Count > 0) {
				field = textboxes.Details[0];
				textboxes.FocusField = field;
				Console.SetCursorPosition(field.Location.X + field.Text.Length, field.Location.Y);
				Console.CursorVisible = true;
			// Otherwise, hide the cursor.
			} else 
				 Console.CursorVisible = false;
			labels.Rendered();
			textboxes.Rendered();

			if (keyThread.Name == null) {
				// Start the thread that listens for keypresses.
				keyThread.Name = "Keypress loop for " + name;
				keyThread.Start();
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
				// All actions will be against the current field, stored in field
				char cChar = cki.KeyChar;

				if (cChar != 0) { // Guard against unprintable chars.
					var kpea = new KeyPressEventArgs(field, cChar);

					if (keyPressEvent != null)
						keyPressEvent(this, kpea);

					if (!kpea.Cancel) { // Process the keystroke. It wasn't cancelled.
						switch (nKey) {
							case ConsoleKey.Backspace: // Backspace pressed
 								// Is there a character to backspace over?
								if (field.Text.Length > 0) {
									field.Text = field.Text.Substring(0, field.Text.Length - 1);
									Refresh(field);
								}

								break;

							case ConsoleKey.Tab: // Tab -> Move to the next field.
								if (cki.Modifiers == ConsoleModifiers.Shift) { 
									// Go backwards.
									currentField--;

									// If we're at the first field, move to the last.
									if (currentField == -1)
										currentField = textboxes.Count - 1;
								} else {
									// Go forwards
									currentField++;

									// If we're in the last field already, move back to the first.
									if (currentField == textboxes.Count)
										currentField = 0;
								}

								// Set the current field to the next one in the collection.
								field = textboxes.Details[currentField];
								// NOTE: need to implement this when swiching to Details 
								textboxes.FocusField = field;

								// Move the cursor to the location of the next field, accomodating
								// any text that may already be there..
								Console.SetCursorPosition(field.Location.X + field.Text.Length, field.Location.Y);
								break;

							case ConsoleKey.Enter: // Enter -> Fire the complete event if it's wired.
								if (formCompleteEvent != null) {
									var fcea = new FormCompleteEventArgs();

									formCompleteEvent(this, fcea);

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
								if (field != null) {
									// no keystrokes accepted oncd field is full 
									if (field.Text.Length < field.Length) {
										field.NonEventingText += cChar;
										Console.ForegroundColor = field.Foreground;
										Console.BackgroundColor = field.Background;
										// password field uses the password character
										// regular text field shows the actual character.
										if (field.PasswordChar != char.MinValue) Console.Write(field.PasswordChar);
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

		void IDisposable.Dispose() {
			((IDisposable)labels).Dispose();
			((IDisposable)textboxes).Dispose();
			((IDisposable)lines).Dispose();

			// Unwire any listening events.
			keyPressEvent = null;
			formCompleteEvent = null;
			FormCancelled = null;

			// Terminate the keypress loop.
			keyThread.Abort();
			keyThread = null;

			GC.SuppressFinalize(this);
		}

	}

	public class FormCompleteEventArgs : System.EventArgs {
		private bool cancel = false;

		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}

	public class KeyPressEventArgs : System.EventArgs {
		private bool cancel = false;
		private char c = char.MinValue;
		private Textbox field = null;
		public KeyPressEventArgs(Textbox field, char c) {
			this.field = field;
			this.c = c;
		}
		public Textbox Textbox { get { return field; } }
		public char Char { get { return c; } }

		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	
	public class Label : StdConsoleObject {
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
	}

	public class Labels : IEnumerable, IDisposable {
		public List<Label> Details { get; set; }
		private List<Label> labels = new List<Label>();
		// The container of the Label objects.
		public Labels() { }
		public Int32 Count { get { return labels.Count; } }
		internal void Rendered() {
			foreach (Label label in this.Details)
				label.Rendered();

		}
		/*
		internal void Rendered() {
			foreach (StdConsoleObject sco in labels)
				sco.Rendered();
		}
		*/
		public void Add(Label label) {
			labels.Add(label);
		}
		public Label this[string name] {
			get {
				foreach (Label label in labels)
					if (label.Name == name)
						return label;
				return null;
			}
		}

		public Label this[Int32 index] {
			get {
				return labels[index];
			}
		}

		public void Dispose() {
			foreach (IDisposable label in labels)
				label.Dispose();

			labels.Clear();

			GC.SuppressFinalize(this);
		}
		public IEnumerator GetEnumerator() {
			return new LabelEnumerator(labels);
		}
		private class LabelEnumerator : IEnumerator {

			private List<Label> labels = null;
			private Int32 index = -1;
			public LabelEnumerator(List<Label> labels)
			{
				this.labels = labels;
			}
			public object Current {
				get { return labels[index]; }
			}

			public bool MoveNext() {
				return ++index != labels.Count;
			}

			public void Reset() {
				index = -1;
			}
		}

	}
	
	public class Line : IDisposable {
 
		public enum LineOrientation {
			Vertical = 0,
			Horizontal = 1
		}
		private LineOrientation orientation = Line.LineOrientation.Horizontal;
		private ConsoleColor colour;
		private Point location = new Point();
		private Int32 length = 0;
		public Line() { }

		public Line (LineOrientation orientation, ConsoleColor colour, Point location, Int32 length) : this() {
			this.orientation = orientation;
			this.colour = colour;
			this.location = location;
			this.length = length;
		}
		public LineOrientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}

		public ConsoleColor Colour {
			get { return colour; }
			set { colour = value; }
		}

		public Point Location {
			get { return location; }
			set { location = value; }
		}

		public Int32 Length {
			get { return length; }
			set { length = value; }
		}

		void IDisposable.Dispose() {
			((IDisposable)location).Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public class Lines : IEnumerable, IDisposable {
		public List<Line> Details { get; set; }
		private List<Line> lines = new List<Line>();
		public Lines() { }
		public Int32 Count { get { return lines.Count; } }
		public void Add(Line line) {
			lines.Add(line);
		}
		public Line this[Int32 index] {
			get { return lines[index]; }
		}

		void IDisposable.Dispose() {
			foreach (IDisposable line in lines)
				line.Dispose();

			lines.Clear();

			GC.SuppressFinalize(this);
		}

		public IEnumerator GetEnumerator() {
			return new LineEnumerator(lines);
		}

		private class LineEnumerator : IEnumerator {
			private List<Line> lines = null;
			private Int32 index = -1;

			public LineEnumerator(List<Line> lines) {
				this.lines = lines;
			}

			public object Current {
				get { return lines[index]; }
			}

			public bool MoveNext() {
				return ++index != this.lines.Count;
			}

			public void Dispose() {}
			public void Reset() {
				index = -1;
			}
		}
	}	

	public class Point : IDisposable {
		private Int32 x = 0;
		private Int32 y = 0;
		public Point() { }

		public Point(Int32 x, Int32 y) {
			this.x = x;
			this.y = y;
		}
        
		public Int32 X {
			get { return x; }
			set { x = value; }
		}

		public Int32 Y {
			get { return y; }
			set { y = value; }
		}
		public override string ToString() {
			return "X: " + x.ToString() + " Y: " + y.ToString();
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		void IDisposable.Dispose() {
			GC.SuppressFinalize(this);
		}
	}

	public class Textbox : StdConsoleObject {
		private ConsoleColor _DEFAULT_BACKGROUND = ConsoleColor.White;
		private ConsoleColor _DEFAULT_FOREGROUND = ConsoleColor.Black;

		private char passwordChar = char.MinValue;
		private bool focus = false;
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
			get { return focus; }
			set { focus = value; }
		}

		internal string NonEventingText {
			get { return base.text; }
			set { base.	text = value; }
		}
		public char PasswordChar {
			get { return passwordChar; }
			set { passwordChar = value; }
		}
	}

	public class Textboxes : IEnumerable,  IDisposable {
		public List<Textbox> Details { get; set; }
		private List<Textbox> textboxes = new List<Textbox>();
		private Textbox focusField = null;

		public Textboxes() { }
		public Int32 Count { get { return textboxes.Count; } }
		internal void Rendered() {
			foreach (Textbox textbox in this.Details)
				textbox.Rendered();
		}
		/*
		internal void Rendered() {
			foreach (StdConsoleObject sco in textboxes)
				sco.Rendered();
		}
		*/
		internal Textbox FocusField {
			get { return focusField; }
			set {
				if (focusField != value) {
					focusField = value;
					foreach (Textbox t in textboxes)
						t.Focus = (t == focusField);
				} 
			}
		}
		public void Add(Textbox textbox) {
			textboxes.Add(textbox);
		}

		public void ClearValues() {
			foreach (Textbox textbox in textboxes)
				textbox.Text = string.Empty;
		}

		public Textbox this[string name] {
			get {
				foreach (Textbox textbox in textboxes)
					if (textbox.Name == name)
						return textbox;

				return null;
			}
		}

		public Textbox this[Int32 index] {
			get {
				return textboxes[index];
			}
		}

		void IDisposable.Dispose() {
			foreach (IDisposable textbox in textboxes)
				textbox.Dispose();

			textboxes.Clear();

			GC.SuppressFinalize(this);
		}
		public IEnumerator GetEnumerator() {
			return new TextboxEnumerator(textboxes);
		}

		private class TextboxEnumerator : IEnumerator {
			private List<Textbox> textboxes = null;
			private Int32 index = -1;
			public TextboxEnumerator(List<Textbox> textboxes) {
				this.textboxes = textboxes;
			}
			public object Current {
				get { return textboxes[index]; }
			}

			public bool MoveNext() {
				return ++index != textboxes.Count;
			}

			public void Reset() {
				index = -1;
			}
		}
	}

	public abstract class StdConsoleObject : IDisposable {
		private ConsoleColor foreground;
		private ConsoleColor background;
		private Point location = new Point();

		private string name = string.Empty;
		private Int32 length = 0;
		protected bool rendered = false;
		protected string text = string.Empty;
		internal StdConsoleObject() { }

		public StdConsoleObject (string name, Point location, Int32 length) {
			this.name = name;
			this.location = location;
			this.length = length;
		}

		public StdConsoleObject (string name, Point location, Int32 length, string text) : this(name, location, length) {
			this.text = text;
		}

		public StdConsoleObject (string name, Point location, Int32 length, string text, ConsoleColor foreground, ConsoleColor background) : this(name, location, length, text) {
			this.foreground = foreground;
			this.background = background;
		}
		public ConsoleColor Foreground {
			get { return foreground; }
			set { foreground = value; }
		}
		public ConsoleColor Background {
			get { return background; }
			set { background = value; }
		}

		public Point Location {
			get { return location; }
		}

		public string Name {
			get { return name; }
		}

		public string Text {
			get { return text; }
			set {
				if (this.text != value) {
					this.text = value;

					if (rendered) {
						// Mark the starting position and colour of the console.
						Int32 x = Console.CursorLeft;
						Int32 y = Console.CursorTop;

						ConsoleColor fore = Console.ForegroundColor;
						ConsoleColor back = Console.BackgroundColor;

						// Make sure the data being written to the screen is either
						// truncated if too long, or padded if too short, to make the
						// field being shown appear correct.
						string text = this.text;
						if (text.Length > this.length)
							text = text.Substring(0, this.length);

						if (text.Length < this.length)
							text = text.PadRight(this.length, ' ');

						// Actually write the text
						Console.SetCursorPosition(location.X, location.Y);
						Console.BackgroundColor = background;
						Console.ForegroundColor = foreground;
						Console.Write(text);

						// Reset the cursor and colour information.
						Console.ForegroundColor = fore;
						Console.BackgroundColor = back;
						Console.SetCursorPosition(x, y);

						if (this is Textbox)
						if (((Textbox)this).Focus)
							Console.SetCursorPosition(location.X + this.text.Length, location.Y);
					}
				}
			}
		}

		public Int32 Length {
			get { return length; }
		}
		internal void Rendered() {
			rendered = true;
		}
		public void Dispose() {
			((IDisposable)location).Dispose();

			GC.SuppressFinalize(this);
		}
	}
}

