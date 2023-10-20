using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms
{
	/// <summary>
	/// Get input based on automatic interpretation of Data object.
	/// </summary>
	internal class InternalInputDialog : Form
	{
		private const int prefWidth = 340;

		private static readonly Size minSize = new Size(193, 104);

		private static readonly Dictionary<Type, char[]> keyPressValidChars = new Dictionary<Type, char[]>
		{
			[typeof(byte)] = GetCultureChars(true, false, true),
			[typeof(sbyte)] = GetCultureChars(true, true, true),
			[typeof(short)] = GetCultureChars(true, true, true),
			[typeof(ushort)] = GetCultureChars(true, false, true),
			[typeof(int)] = GetCultureChars(true, true, true),
			[typeof(uint)] = GetCultureChars(true, false, true),
			[typeof(long)] = GetCultureChars(true, true, true),
			[typeof(ulong)] = GetCultureChars(true, false, true),
			[typeof(double)] = GetCultureChars(true, true, true, true, true, true),
			[typeof(float)] = GetCultureChars(true, true, true, true, true, true),
			[typeof(decimal)] = GetCultureChars(true, true, true, true, true),
			[typeof(TimeSpan)] = GetCultureChars(true, true, false, new char[] { '-' }),
			[typeof(Guid)] = GetCultureChars(true, false, false, "-{}()".ToCharArray()),
		};

		private static readonly Type[] simpleTypes = new Type[] { typeof(Enum), typeof(Decimal), typeof(DateTime),
			typeof(DateTimeOffset), typeof(String), typeof(TimeSpan), typeof(Guid) };

		private static readonly Dictionary<Type, Predicate<string>> validations = new Dictionary<Type, Predicate<string>>
		{
			[typeof(byte)] = s => { byte n; return byte.TryParse(s, out n); },
			[typeof(sbyte)] = s => { sbyte n; return sbyte.TryParse(s, out n); },
			[typeof(short)] = s => { short n; return short.TryParse(s, out n); },
			[typeof(ushort)] = s => { ushort n; return ushort.TryParse(s, out n); },
			[typeof(int)] = s => { int n; return int.TryParse(s, out n); },
			[typeof(uint)] = s => { uint n; return uint.TryParse(s, out n); },
			[typeof(long)] = s => { long n; return long.TryParse(s, out n); },
			[typeof(ulong)] = s => { ulong n; return ulong.TryParse(s, out n); },
			[typeof(char)] = s => { char n; return char.TryParse(s, out n); },
			[typeof(double)] = s => { double n; return double.TryParse(s, out n); },
			[typeof(float)] = s => { float n; return float.TryParse(s, out n); },
			[typeof(decimal)] = s => { decimal n; return decimal.TryParse(s, out n); },
			[typeof(DateTime)] = s => { DateTime n; return DateTime.TryParse(s, out n); },
			[typeof(TimeSpan)] = s => { TimeSpan n; return TimeSpan.TryParse(s, out n); },
			[typeof(Guid)] = s => { try { Guid n = new Guid(s); return true; } catch { return false; } },
		};

		private Panel borderPanel;
		private TableLayoutPanel buttonPanel;
		private Button cancelBtn;
		private IContainer components;
		private object dataObj;
		private ErrorProvider errorProvider;
		private Image image;
		private List<MemberInfo> items = new List<MemberInfo>();
		private Button okBtn;
		private string prompt;
		private TableLayoutPanel table;

		/// <summary>
		/// Initializes a new instance of the <see cref="InternalInputDialog"/> class.
		/// </summary>
		public InternalInputDialog()
		{
			InitializeComponent();
		}

		internal InternalInputDialog(string prompt, string caption, Image image, object data, int width) : this()
		{
			this.Width = width;
			this.prompt = prompt;
			this.Text = caption;
			this.image = image;
			this.Data = data;
		}

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Data
		{
			get { return dataObj; }
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(Data));

				items.Clear();

				if (IsSimpleType(value.GetType()))
					items.Add(null);
				else
				{
					foreach (var mi in value.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public))
					{
						if (GetAttr(mi)?.Hidden ?? false)
							continue;
						var fi = mi as FieldInfo;
						var pi = mi as PropertyInfo;
						if (fi != null && IsSupportedType(fi.FieldType))
						{
							items.Add(fi);
						}
						else if (pi != null && IsSupportedType(pi.PropertyType) && pi.GetIndexParameters().Length == 0 && pi.CanWrite)
						{
							items.Add(pi);
						}
					}

					items.Sort((x, y) => (GetAttr(x)?.Order ?? int.MaxValue) - (GetAttr(y)?.Order ?? int.MaxValue));
				}

				dataObj = value;

				BuildTable();
			}
		}

		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>
		/// The image.
		/// </value>
		[DefaultValue(null)]
		public Image Image
		{
			get { return image; }
			set
			{
				if (image != value)
				{
					image = value;
					BuildTable();
				}
			}
		}

		/// <summary>
		/// Gets or sets the prompt.
		/// </summary>
		/// <value>
		/// The prompt.
		/// </value>
		[DefaultValue(null)]
		public string Prompt
		{
			get { return prompt; }
			set
			{
				if (prompt != value)
				{
					prompt = value;
					BuildTable();
				}
			}
		}

		public new int Width
		{
			get { return base.Width; }
			set
			{
				if (value == 0) value = prefWidth;
				value = Math.Max(minSize.Width, value);
				base.MinimumSize = new Size(value, minSize.Height);
				base.MaximumSize = new Size(value, int.MaxValue);
			}
		}

		private bool HasPrompt => !string.IsNullOrEmpty(Prompt);

		private static object ConvertFromStr(string value, Type destType)
		{
			if (destType == typeof(string))
				return value;
			if (value.Trim() == string.Empty)
				return destType.IsValueType ? Activator.CreateInstance(destType) : null;
			if (typeof(IConvertible).IsAssignableFrom(destType))
				try { return Convert.ChangeType(value, destType); } catch { }
			return TypeDescriptor.GetConverter(destType).ConvertFrom(value);
		}

		private static string ConvertToStr(object value)
		{
			if (value == null)
				return string.Empty;
			IConvertible conv = value as IConvertible;
			if (conv != null)
				return value.ToString();
			return (string)TypeDescriptor.GetConverter(value).ConvertTo(value, typeof(string));
		}

		private static int GetBestHeight(Control c)
		{
			using (Graphics g = c.CreateGraphics())
				return TextRenderer.MeasureText(g, c.Text, c.Font, new Size(c.Width, 0), TextFormatFlags.WordBreak).Height;
		}

		private static char[] GetCultureChars(bool digits, bool neg, bool pos, bool dec = false, bool grp = false, bool e = false)
		{
			var c = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
			var l = new List<string>();
			if (digits) l.AddRange(c.NativeDigits);
			if (neg) l.Add(c.NegativeSign);
			if (pos) l.Add(c.PositiveSign);
			if (dec) l.Add(c.NumberDecimalSeparator);
			if (grp) l.Add(c.NumberGroupSeparator);
			if (e) l.Add("Ee");
			var sb = new System.Text.StringBuilder();
			foreach (var s in l)
				sb.Append(s);
			char[] ca = sb.ToString().ToCharArray();
			Array.Sort(ca);
			return ca;
		}

		private static char[] GetCultureChars(bool timeChars, bool timeSep, bool dateSep, char[] other)
		{
			var c = System.Globalization.CultureInfo.CurrentCulture;
			var l = new List<string>();
			if (timeChars) l.AddRange(c.NumberFormat.NativeDigits);
			if (timeSep) { l.Add(c.DateTimeFormat.TimeSeparator); l.Add(c.NumberFormat.NumberDecimalSeparator); }
			if (dateSep) l.Add(c.DateTimeFormat.DateSeparator);
			if (other != null && other.Length > 0) l.Add(new string(other));
			var sb = new System.Text.StringBuilder();
			foreach (var s in l)
				sb.Append(s);
			char[] ca = sb.ToString().ToCharArray();
			Array.Sort(ca);
			return ca;
		}

		private static bool IsSimpleType(Type type) => type.IsPrimitive || type.IsEnum || Array.Exists(simpleTypes, t => t == type) || Convert.GetTypeCode(type) != TypeCode.Object ||
			(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]));

		private static bool IsSupportedType(Type type)
		{
			if (typeof(IConvertible).IsAssignableFrom(type))
				return true;
			var cvtr = TypeDescriptor.GetConverter(type);
			if (cvtr.CanConvertFrom(typeof(string)) && cvtr.CanConvertTo(typeof(string)))
				return true;
			return false;
		}

		/// <summary>
		/// Binds input text values back to the Data object.
		/// </summary>
		private void BindToData()
		{
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				var itemType = GetItemType(item);

				// Get value from control
				Control c = table.Controls[$"input{i}"];
				object val;
				if (c is CheckBox)
					val = ((CheckBox)c).Checked;
				else
					val = ConvertFromStr(c.Text, itemType);

				// Apply value to dataObj
				if (item == null)
					dataObj = val;
				else if (item is PropertyInfo)
					((PropertyInfo)item).SetValue(dataObj, val, null);
				else
					((FieldInfo)item).SetValue(dataObj, val);
			}
		}

		private Control BuildInputForItem(int i)
		{
			var item = items[i];
			var itemType = GetItemType(item);

			// Get default text value
			object val;
			if (item == null)
				val = dataObj;
			else if (item is PropertyInfo)
				val = ((PropertyInfo)item).GetValue(dataObj, null);
			else
				val = ((FieldInfo)item).GetValue(dataObj);
			string t = ConvertToStr(val);

			// Build control type
			Control retVal;
			if (itemType == typeof(bool))
			{
				retVal = new CheckBox { AutoSize = true, Checked = (bool)val, Margin = new Padding(0, 7, 0, 0), MinimumSize = new Size(0, 20) };
			}
			else if (itemType.IsEnum)
			{
				var cb = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
				cb.Items.AddRange(Enum.GetNames(itemType));
				cb.Text = t;
				retVal = cb;
			}
			else
			{
				var tb = new TextBox { CausesValidation = true, Dock = DockStyle.Fill, Text = t };
				tb.Enter += (s, e) => tb.SelectAll();
				if (itemType == typeof(char))
					tb.KeyPress += (s, e) => e.Handled = !char.IsControl(e.KeyChar) && tb.TextLength > 0;
				else
					tb.KeyPress += (s, e) => e.Handled = IsInvalidKey(e.KeyChar, itemType);
				tb.Validating += (s, e) =>
				{
					bool invalid = TextIsInvalid(tb, itemType);
					e.Cancel = invalid;
					errorProvider.SetError(tb, invalid ? $"Text must be in a valid format for {itemType.Name}." : "");
				};
				tb.Validated += (s, e) => errorProvider.SetError(tb, "");
				errorProvider.SetIconPadding(tb, -18);
				retVal = tb;
			}

			// Set standard props
			// TODO: Change out '7' for DPI specific spacing
			retVal.Margin = new Padding(items.Count == 1 && HasPrompt && items[0] == null ? 4 : 0, 7, 0, 0);
			retVal.Name = $"input{i}";
			return retVal;
		}

		private Label BuildLabelForItem(int i)
		{
			var item = items[i];
			var lbl = new Label { AutoSize = true, Dock = DockStyle.Left, Margin = new Padding(0, 0, 1, 0) };
			if (item != null)
			{
				lbl.Text = (GetAttr(item)?.Label ?? item.Name) + ":";
				// TODO: Change out '10' for spacing needed to align label text with TextBox and '4' for DPI specific spacing
				lbl.Margin = new Padding(0, 10, 4, 0);
			}
			return lbl;
		}

		private void BuildTable()
		{
			table.SuspendLayout();

			// Clear out last layout
			table.Controls.Clear();
			while (table.RowStyles.Count > 1)
				table.RowStyles.RemoveAt(1);

			table.RowCount = items.Count + (HasPrompt ? 1 : 0);

			// Icon
			if (Image != null)
			{
				table.Controls.Add(new PictureBox { Image = this.Image, Size = this.Image.Size, Margin = new Padding(0, 0, 7, 0), TabStop = false }, 0, 0);
				table.SetRowSpan(table.GetControlFromPosition(0, 0), table.RowCount);
			}

			int hrow = 0;

			// Add header row if needed
			Label lbl = null;
			if (HasPrompt)
			{
				lbl = new Label { AutoSize = true, Text = Prompt, Dock = DockStyle.Top, UseMnemonic = false };
				lbl.Font = new Font(Font.FontFamily, Font.Size * 4 / 3);
				lbl.ForeColor = Color.FromArgb(19, 112, 171);
				lbl.Margin = new Padding(items.Count == 1 && items[0] == null ? 1 : 0, 0, 0, 0);
				table.Controls.Add(lbl, 1, hrow++);
				table.SetColumnSpan(lbl, 2);
			}

			// Build rows for each item
			for (int i = 0; i < items.Count; i++)
			{
				if (i + hrow > 0)
					table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				table.Controls.Add(BuildLabelForItem(i), 1, i + hrow);
				table.Controls.Add(BuildInputForItem(i), 2, i + hrow);
			}

			table.ResumeLayout();

			if (HasPrompt)
			{
				if (lbl.PreferredWidth > lbl.Width)
					lbl.MinimumSize = lbl.Size;
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private InputDialogItemAttribute GetAttr(MemberInfo mi) => (InputDialogItemAttribute)System.Attribute.GetCustomAttribute(mi, typeof(InputDialogItemAttribute), true);

		private Type GetItemType(MemberInfo mi) => mi == null ? dataObj.GetType() : (mi is PropertyInfo ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType);

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();
			this.okBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.borderPanel = new System.Windows.Forms.Panel();
			this.table = new System.Windows.Forms.TableLayoutPanel();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.buttonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			//
			// buttonPanel
			//
			this.buttonPanel.AutoSize = true;
			this.buttonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonPanel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonPanel.ColumnCount = 3;
			this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.buttonPanel.Controls.Add(this.okBtn, 1, 0);
			this.buttonPanel.Controls.Add(this.cancelBtn, 2, 0);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonPanel.Location = new System.Drawing.Point(0, 25);
			this.buttonPanel.Margin = new System.Windows.Forms.Padding(0);
			this.buttonPanel.MinimumSize = new System.Drawing.Size(177, 40);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 9);
			this.buttonPanel.RowCount = 1;
			this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.buttonPanel.Size = new System.Drawing.Size(177, 40);
			this.buttonPanel.TabIndex = 1;
			//
			// okBtn
			//
			this.okBtn.Location = new System.Drawing.Point(10, 8);
			this.okBtn.Margin = new System.Windows.Forms.Padding(0, 0, 7, 0);
			this.okBtn.MinimumSize = new System.Drawing.Size(75, 23);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 0;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			//
			// cancelBtn
			//
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Location = new System.Drawing.Point(92, 8);
			this.cancelBtn.Margin = new System.Windows.Forms.Padding(0);
			this.cancelBtn.MinimumSize = new System.Drawing.Size(75, 23);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(75, 23);
			this.cancelBtn.TabIndex = 1;
			this.cancelBtn.Text = "&Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			//
			// borderPanel
			//
			this.borderPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
			this.borderPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.borderPanel.Location = new System.Drawing.Point(0, 24);
			this.borderPanel.Margin = new System.Windows.Forms.Padding(0);
			this.borderPanel.MinimumSize = new System.Drawing.Size(0, 1);
			this.borderPanel.Name = "borderPanel";
			this.borderPanel.Size = new System.Drawing.Size(177, 1);
			this.borderPanel.TabIndex = 2;
			//
			// table
			//
			this.table.AutoSize = true;
			this.table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.table.ColumnCount = 3;
			this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.table.Dock = System.Windows.Forms.DockStyle.Fill;
			this.table.Location = new System.Drawing.Point(0, 0);
			this.table.Margin = new System.Windows.Forms.Padding(0);
			this.table.Name = "table";
			this.table.Padding = new System.Windows.Forms.Padding(10);
			this.table.RowCount = 1;
			this.table.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.table.Size = new System.Drawing.Size(177, 24);
			this.table.TabIndex = 3;
			//
			// errorProvider
			//
			this.errorProvider.ContainerControl = this;
			//
			// InternalInputDialog
			//
			this.AcceptButton = this.okBtn;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = this.cancelBtn;
			this.ClientSize = new System.Drawing.Size(prefWidth, 65);
			this.Controls.Add(this.table);
			this.Controls.Add(this.borderPanel);
			this.Controls.Add(this.buttonPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimumSize = new System.Drawing.Size(prefWidth, minSize.Height);
			this.MaximumSize = new System.Drawing.Size(prefWidth, int.MaxValue);
			this.Name = "InternalInputDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.buttonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private bool IsInvalidKey(char keyChar, Type itemType)
		{
			if (char.IsControl(keyChar))
				return false;
			char[] chars;
			keyPressValidChars.TryGetValue(itemType, out chars);
			if (chars != null)
			{
				int si = Array.BinarySearch<char>(chars, keyChar);
				System.Diagnostics.Debug.WriteLine($"Processed key {keyChar} as {si} position.");
				if (si < 0)
					return true;
			}
			return false;
		}

		private void okBtn_Click(object sender, EventArgs e)
		{
			BindToData();
			DialogResult = DialogResult.OK;
			Close();
		}

		private bool TextIsInvalid(TextBox tb, Type itemType)
		{
			if (string.IsNullOrEmpty(tb.Text))
				return false;
			Predicate<string> p;
			validations.TryGetValue(itemType, out p);
			if (p != null)
				return !p(tb.Text);
			return false;
		}

		private class RegexTextBox : TextBox
		{
			public string RegexPattern { get; set; }

			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				//System.Text.RegularExpressions.Regex.IsMatch()
				base.OnKeyPress(e);
			}
		}
	}

	/// <summary>
	/// An input dialog that automatically creates controls to collect the values of the object supplied via the <see cref="Data"/> property.
	/// </summary>
	public class InputDialog : CommonDialog
	{
		private object data;

		/// <summary>
		/// Gets or sets the data for the input dialog box. The data type will determine the type of input mechanism displayed. For simple
		/// types, a <see cref="TextBox"/> with validation, or a <see cref="CheckBox"/> or a <see cref="ComboBox"/> will
		/// be displayed. For classes and structures, all of the public, top-level, fields and properties will have
		/// input mechanisms shown for each. See Remarks for more detail.
		/// </summary>
		/// <value>
		/// The data for the input dialog box.
		/// </value>
		/// <remarks>TBD</remarks>
		[DefaultValue(null), Category("Data"), Description("The data for the input dialog box.")]
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		/// <summary>
		/// Gets or sets the image to display on the top left corner of the dialog. This value can be <c>null</c> to display no image.
		/// </summary>
		/// <value>
		/// The image to display on the top left corner of the dialog.
		/// </value>
		[DefaultValue(null), Category("Appearance"), Description("The image to display on the top left corner of the dialog.")]
		public Image Image { get; set; }

		/// <summary>
		/// Gets or sets the text prompt to display above all input options. This value can be <c>null</c>.
		/// </summary>
		/// <value>
		/// The text prompt to display above all input options.
		/// </value>
		[DefaultValue(null), Category("Appearance"), Description("The text prompt to display above all input options.")]
		public string Prompt { get; set; }

		/// <summary>
		/// Gets or sets the input dialog box title.
		/// </summary>
		/// <value>
		/// The input dialog box title. The default value is an empty string ("").
		/// </value>
		[DefaultValue(""), Category("Window"), Description("The input dialog box title.")]
		public string Title { get; set; } = "";

		/// <summary>
		/// Displays an input dialog in front of the specified object and with the specified prompt, caption, data, and image.
		/// </summary>
		/// <param name="owner">An implementation of <see cref="IWin32Window"/> that will own the modal dialog box.</param>
		/// <param name="prompt">The text prompt to display above all input options. This value can be <c>null</c>.</param>
		/// <param name="caption">The caption for the dialog.</param>
		/// <param name="data">
		/// The data for the input. The data type will determine the type of input mechanism displayed. For simple
		/// types, a <see cref="TextBox"/> with validation, or a <see cref="CheckBox"/> or a <see cref="ComboBox"/> will
		/// be displayed. For classes and structures, all of the public, top-level, fields and properties will have
		/// input mechanisms shown for each. See Remarks for more detail.
		/// </param>
		/// <param name="image">
		/// The image to display on the top left corner of the dialog. This value can be <c>null</c> to display no image.
		/// </param>
		/// <param name="width">
		/// The desired width of the <see cref="InternalInputDialog"/>. A value of <c>0</c> indicates a default width.
		/// </param>
		/// <returns>
		/// Either <see cref="DialogResult.OK"/> or <see cref="DialogResult.Cancel"/>. On OK, the
		/// <paramref name="data"/> parameter will include the updated values from the <see cref="InternalInputDialog"/>.
		/// </returns>
		/// <remarks></remarks>
		public static DialogResult Show(IWin32Window owner, string prompt, string caption, ref object data, System.Drawing.Image image = null, int width = 0)
		{
			using (var dlg = new InternalInputDialog(prompt, caption, image, data, width))
			{
				var ret = owner == null ? dlg.ShowDialog() : dlg.ShowDialog(owner);
				if (ret == DialogResult.OK)
					data = dlg.Data;
				return ret;
			}
		}

		/// <summary>
		/// Displays an input dialog with the specified prompt, caption, data, and image.
		/// </summary>
		/// <param name="prompt">The text prompt to display above all input options. This value can be <c>null</c>.</param>
		/// <param name="caption">The caption for the dialog.</param>
		/// <param name="data">
		/// The data for the input. The data type will determine the type of input mechanism displayed. For simple
		/// types, a <see cref="TextBox"/> with validation, or a <see cref="CheckBox"/> or a <see cref="ComboBox"/> will
		/// be displayed. For classes and structures, all of the public, top-level, fields and properties will have
		/// input mechanisms shown for each. See Remarks for more detail.
		/// </param>
		/// <param name="image">
		/// The image to display on the top left corner of the dialog. This value can be <c>null</c> to display no image.
		/// </param>
		/// <param name="width">
		/// The desired width of the <see cref="InternalInputDialog"/>. A value of <c>0</c> indicates a default width.
		/// </param>
		/// <returns>
		/// Either <see cref="DialogResult.OK"/> or <see cref="DialogResult.Cancel"/>. On OK, the
		/// <paramref name="data"/> parameter will include the updated values from the <see cref="InternalInputDialog"/>.
		/// </returns>
		/// <remarks></remarks>
		public static DialogResult Show(string prompt, string caption, ref object data, System.Drawing.Image image = null, int width = 0) => Show(null, prompt, caption, ref data, image, width);

		/// <summary>
		/// Resets all properties to their default values.
		/// </summary>
		public override void Reset() { }

		/// <summary>
		/// <para>This API supports the.NET Framework infrastructure and is not intended to be used directly from your code.</para>
		/// <para>Specifies a common dialog box.</para>
		/// </summary>
		/// <param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box.</param>
		/// <returns><c>true</c> if the data was collected; otherwise, <c>false</c>.</returns>
		protected override bool RunDialog(IntPtr hwndOwner) => Show(NativeWindow.FromHandle(hwndOwner), Prompt, Title, ref data, Image) == DialogResult.OK;
	}

	/// <summary>
	/// Allows a developer to attribute a property or field with text that gets shown instead of the field or property name in an <see cref="InputDialog"/>.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class InputDialogItemAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InputDialogItemAttribute" /> class.
		/// </summary>
		public InputDialogItemAttribute() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="InputDialogItemAttribute" /> class.
		/// </summary>
		/// <param name="label">The label to use in the <see cref="InputDialog"/> as the label for this field or property.</param>
		public InputDialogItemAttribute(string label)
		{
			Label = label;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this item is hidden and not displayed by the <see cref="InputDialog"/>.
		/// </summary>
		/// <value>
		/// <c>true</c> if hidden; otherwise, <c>false</c>.
		/// </value>
		public bool Hidden { get; set; } = false;

		/// <summary>
		/// Gets or sets the label to use in the <see cref="InputDialog"/> as the label for this field or property.
		/// </summary>
		/// <value>
		/// The label for this item.
		/// </value>
		public string Label { get; } = null;

		/// <summary>
		/// Gets or sets the order in which to display the input for this field or property within the <see cref="InputDialog"/>.
		/// </summary>
		/// <value>
		/// The display order for this item.
		/// </value>
		public int Order { get; set; } = int.MaxValue;
	}
}