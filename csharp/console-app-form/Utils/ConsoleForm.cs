using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Utils {
	public class ConsoleForm : System.IDisposable{
		public delegate void onFormComplete(ConsoleForm sender, FormCompleteEventArgs e);

		public delegate void onKeyPress(ConsoleForm sender, KeyPressEventArgs e);
		public delegate void onFormCancelled(ConsoleForm sender, System.EventArgs e);
		public event onFormCancelled FormCancelled;

		private onKeyPress keyPressEvent = null;
		private Lines lines = new Lines();
		private Labels labels = new Labels();
		private Textboxes textboxes = new Textboxes();
		private string name = string.Empty;
		private Int32 width = 80;
		private Int32 height = 40;
		private Thread keyThread = null;
		private ThreadStart keyThreadStart = null;
		private Textbox field = null;
		private bool debug;
		public Boolean Debug { get { return debug; } set { debug = value; } }

		public void start() {
			// Initialize the keypress thread variables.
			keyThreadStart = new ThreadStart(LoopForKeypress);
			keyThread = new Thread(keyThreadStart);
		}

		// TODO: are these costructors needed
		public ConsoleForm() {
			// Initialize the keypress thread variables.
			keyThreadStart = new ThreadStart(LoopForKeypress);
			keyThread = new Thread(keyThreadStart);
		}
		
		public ConsoleForm(Int32 width, Int32 height) : this() {
			this.width = width;
			this.height = height;
		}
		~ConsoleForm() { }
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

		public static ConsoleForm GetFormInstance(string path) {
			var json = getFileContents(path);
			ConsoleForm form = JsonConvert.DeserializeObject<ConsoleForm>(json);
			return form;
		}

		public static ConsoleForm GetFormInstance(string path, onFormComplete formComplete, onFormCancelled formCancelled) {
			// Call the other static factory method to get a new console form.
			var form = ConsoleForm.GetFormInstance(path);

			if (formComplete != null)
				form.FormComplete += formComplete;

			if (formCancelled != null)
				form.FormCancelled += formCancelled;

			return form;
		}
			
		public void  Dispose() { }
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
			// Now draw the textboxes.
			foreach (Textbox text in textboxes.Details)
				Refresh(text);

			if (keyThread.Name == null) {
				// Start the thread that listens for keypresses.
				keyThread.Name = "Keypress loop for " + name;
				keyThread.Start();
			}
			
		}

		private void Refresh(Textbox textbox) {
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
			if (textbox.PasswordChar != char.MinValue)
				text = new string(textbox.PasswordChar, textbox.Text.Length);
			else
				text = textbox.Text;

			if (text.Length > textbox.Length)
				text = text.Substring(0, textbox.Length);

			if (text.Length < textbox.Length)
				text = text.PadRight(textbox.Length, ' ');

			// Actually write the text
			Console.SetCursorPosition(textbox.Location.X, textbox.Location.Y);
			Console.BackgroundColor = textbox.Background;
			Console.ForegroundColor = textbox.Foreground;
			Console.Write(text);

			// Reset the cursor and colour information.
			Console.ForegroundColor = fore;
			Console.BackgroundColor = back;
			Console.SetCursorPosition(x, y);

			// If the field being updated is the "current" field (i.e. the one with the 
			// cursor in it), reposition the cursor to accomodate existing text.
			if (textbox == field)
				Console.SetCursorPosition(field.Location.X + field.Text.Length, 
					field.Location.Y);
		}
		
		private void Refresh(StdConsoleObject stdConsoleObject) {
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
			if (stdConsoleObject is Textbox && ((Textbox)stdConsoleObject).PasswordChar != char.MinValue)
				text = new string(((Textbox)stdConsoleObject).PasswordChar, stdConsoleObject.Text.Length);
			else
				text = stdConsoleObject.Text;

			if (text.Length > stdConsoleObject.Length)
				text = text.Substring(0, stdConsoleObject.Length);

			if (text.Length < stdConsoleObject.Length)
				text = text.PadRight(stdConsoleObject.Length, ' ');

			// Actually write the text
			Console.SetCursorPosition(stdConsoleObject.Location.X, stdConsoleObject.Location.Y);
			Console.BackgroundColor = stdConsoleObject.Background;
			Console.ForegroundColor = stdConsoleObject.Foreground;
			Console.Write(text);

			// Reset the cursor and colour information.
			Console.ForegroundColor = fore;
			Console.BackgroundColor = back;
			Console.SetCursorPosition(x, y);

			// If the field being updated is the "current" field (i.e. the one with the 
			// cursor in it), reposition the cursor to accomodate existing text.
			if (stdConsoleObject is Textbox)
			if (((Textbox)stdConsoleObject) == field)
				Console.SetCursorPosition(field.Location.X + field.Text.Length, 
					field.Location.Y);
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
					var kpea = new KeyPressEventArgs(field, cChar);

					if (keyPressEvent != null)
						keyPressEvent(this, kpea);

					if (!kpea.Cancel) { // Process the keystroke. It wasn't cancelled.
						switch (nKey) {
							default:
								if (field != null) {
									// no keystrokes accepted oncd field is full 
										Console.Write(cChar);
								}
								break;
						}
					}
				}
			}
		}

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
		
	}


	public class Line {
 
		public enum LineOrientation {
			Vertical = 0,
			Horizontal = 1
		}
		private LineOrientation orientation = Line.LineOrientation.Horizontal;
		private ConsoleColor colour;
		private Point location = new Point();
		private Int32 length = 0;
		public Line()
		{
		}

		public Line(LineOrientation orientation, ConsoleColor colour, Point location, Int32 length)
			: this()
		{
			this.orientation = orientation;
			this.colour = colour;
			this.location = location;
			this.length = length;
		}

		public LineOrientation Orientation { get { return orientation; } set { orientation = value; } }

		public ConsoleColor Colour { get { return colour; } set { colour = value; } }

		public Point Location { get { return location; } set { location = value; } }

		public Int32 Length { get { return length; } set { length = value; } }
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
	
	public class Lines
	{
		public List<Line> Details { get; set; }
	}

	public class Labels {
		public List<Label> Details { get; set; }
	}

	public class Textboxes {
		public List<Textbox> Details { get; set; }
	}


	public class Label {
		private ConsoleColor _DEFAULT_BACKGROUND = ConsoleColor.Black;
		private ConsoleColor _DEFAULT_FOREGROUND = ConsoleColor.White;
		private Point location = new Point();
	}

	public class Point {
		private Int32 x = 0;
		private Int32 y = 0;
		public Point()
		{
		}

		public Point(Int32 x, Int32 y)
		{
			this.x = x;
			this.y = y;
		}
        
		public Int32 X { get { return x; } set { x = value; } }

		public Int32 Y { get { return y; } set { y = value; } }

		public override string ToString()
		{
			return "X: " + x.ToString() + " Y: " + y.ToString();
		}

		// TODO: redundane method override
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}


	public abstract class StdConsoleObject : IDisposable {
		private ConsoleColor foreground;
		private ConsoleColor background;

		protected Point location = new Point();

		protected string name = string.Empty;
		protected Int32 length = 0;
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
		public ConsoleColor Foreground { get { return foreground; } set { foreground = value; } }
		public ConsoleColor Background { get { return background; } set { background = value; } 		}
		public Point Location { get { return location; } set { location = value; } }
		public string Name { get { return name; } set { name = value; } }

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

	public class Textbox : StdConsoleObject {
		private ConsoleColor _DEFAULT_BACKGROUND = ConsoleColor.White;
		private ConsoleColor _DEFAULT_FOREGROUND = ConsoleColor.Black;
		private ConsoleColor foreColour;
		private ConsoleColor backColour;
		public ConsoleColor ForeColour { get { return foreColour; } set { foreColour = value; } }

		public ConsoleColor BackColour { get { return backColour; } set { backColour = value; } }

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
		internal bool Focus { get { return focus; } set { focus = value; } }

		internal string NonEventingText { get { return base.text; } set { base.text = value; } }
		public char PasswordChar { get { return passwordChar; } set { passwordChar = value; } }
		public Point Location { get { return base.Location; } set { base.Location = value; } }
		public string Text { get { return base.Text; } set { base.Text = value; } } 
 	}

}


