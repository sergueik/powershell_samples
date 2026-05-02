#Copyright (c) 2026 Serguei Kouzmine
#Permission is hereby granted, free of charge, to any person obtaining a copy
#of this software and associated documentation files (the "Software"), to deal
#in the Software without restriction, including without limitation the rights
#to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#copies of the Software, and to permit persons to whom the Software is
#furnished to do so, subject to the following conditions:
#The above copyright notice and this permission notice shall be included in
#all copies or substantial portions of the Software.
#THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#THE SOFTWARE.


param (
  # [int] $processid = 42068,
  # [string] $value = 'example.Application',
  # [string] $name = 'java.exe',
  [string] $category = 'Memory',
  [string] $counter = 'Available Bytes',
  [string] $instance = '', # 'java'
  [switch] $debug # currently unused
)

[bool]$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'
# TODO: enable
# $debug_flag = $true
# for running commandline-less Powershell ISE
if ($debug_flag){
  $DebugPreference = 'Continue'
}

# http://www.codeproject.com/Articles/39235/Circular-Progress-Control-Mac-OS-X-style

Add-Type -TypeDefinition @"

// "
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

using System.Timers;
using System.Windows.Forms;
using System;
using System.ComponentModel;

namespace Utils {
	public class UserControl1 : UserControl {
        	private string result;
		private Boolean debug;
		private CircularBuffer<Data> buffer;

		private string dataFile = @"c:\temp\loadaveragecounterservice.txt";
		private Random rand = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));
		private int rounds = 100;
		private int averageInterval = 60000;
		private int collectInterval = 1000;
		private static int capacity = 900;
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";

		public string CategoryName {
			get { return categoryName; }
			set  { categoryName = value; }
		}
		public string CounterName {
			get { return counterName; }
			set  { counterName = value; }
		}
		public string InstanceName {
			get { return instanceName; }
			set  { instanceName = value; }
		}
		public string DataFile {
			get { return dataFile; }
			set  { dataFile = value; }
		}
		public int AverageInterval {
			get { return averageInterval; }
			set  { averageInterval = value; }
		}
		public int CollectInterval {
			get { return collectInterval; }
			set  { collectInterval = value; }
		}
		public int Capacity {
			get { return capacity; }
			set  { capacity = value; }
		}
		public int Rounds {
			get { return rounds; }
			set  { rounds = value; }
		}
		public Boolean Debug {
			get { return debug; }
			set  { debug = value; }
		}

		public UserControl1() {
			buffer = new CircularBuffer<Data>(capacity);
			// appSettings = ConfigurationManager.AppSettings;
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e) {
			var thread = new Thread(new ThreadStart(StartCalculation));
			thread.Start();

			timer1.Interval = collectInterval;
			timer1.Enabled = true;
			timer1.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => CollectMetrics());
			timer1.Start();

			timer2.Interval = averageInterval;
			timer2.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => Commit());
			timer2.Enabled = true;
			timer2.Start();

		}

		private Action<Control> setEnabled = (Control control) => control.Enabled = true;
		public void StartCalculation() {
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.control.invoke?view=netframework-4.5
			// https://learn.microsoft.com/en-us/dotnet/api/system.delegate?view=netframework-4.5
			button1.safeInvoke((Control control) => control.Enabled = false);

			for (int i = 0; i <= rounds; i++) {
				Thread.Sleep(collectInterval);
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.Label?view=netframework-4.5
				label1.safeInvoke((Label label) => label.Text = ((float)i / 10).ToString("F2") + " %");
				// https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.ProgressBar?view=netframework-4.5
				progressBar1.safeInvoke((ProgressBar progressBar) => progressBar.Value = i);
				string labelText = label1.safeInvoke((Label label) => label.Text);
			}
			button1.safeInvoke((Control control) => control.Enabled = true);

			if (timer2 != null) {
				timer1.Stop();
				timer1.Enabled = false;
			}
			if (timer2 != null) {
				timer2.Stop();
				timer2.Enabled = false;
			}
		}

 		private void CollectMetrics() {
			float value = 0;
			var row = new Data();
			row.TimeStamp = DateTime.Now;
			try {
				// https://learn.microsoft.com/en-us/windows/win32/perfctrs/performance-counters-reference
				var performanceCounter = new PerformanceCounter();
				performanceCounter.CategoryName = this.categoryName;
				performanceCounter.CounterName = this.counterName;
				performanceCounter.InstanceName = this.instanceName == "" ? null : this.InstanceName;
				// value = (long)performanceCounter.RawValue;
				value = performanceCounter.NextValue();
			} catch (InvalidOperationException e) {
				if (this.debug)
					Console.Error.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", this.categoryName, this.counterName, "0", e.ToString()));
				return;
			}
			row.Value = value;
			buffer.AddLast(row);
		}

		private void Commit() {

			var rows = buffer.ToList();
			var now = DateTime.Now;
			double average = 0;
			IEnumerable<float> values = null;

			try {
				values = (from row in rows
				          where ((now - row.TimeStamp).TotalMilliseconds) <= (float)this.averageInterval
				          select row.Value);
				average = values.Average();
				this.result = String.Format("{0} from {1} samples", average, values.Count());
				if (this.debug)
					Console.Error.WriteLine(this.result);

			} catch (Exception e) {
				// System.InvalidOperationException: Sequence contains no elements
				if (this.debug)
					Console.Error.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
		}

		private System.ComponentModel.IContainer components = null;
		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			if (timer1 != null) {
				timer1.Stop();
				timer1.Enabled = false;
			}
			if (timer2 != null) {
				timer2.Stop();
				timer2.Enabled = false;
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			timer1 = new System.Timers.Timer();
			timer2 = new System.Timers.Timer();
			timer1.Enabled = true;
			timer1.Interval = 1000D;
			timer1.SynchronizingObject = this;

			timer2.Enabled = true;
			timer2.Interval = 60000D;
			timer2.SynchronizingObject = this;

			button1 = new Button();
			progressBar1 = new ProgressBar();
			label1 = new Label();
			this.SuspendLayout();

			button1.Location = new Point(26, 22);
			button1.Margin = new Padding(6);
			button1.Name = "button1";
			button1.Size = new Size(182, 42);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(26, 76);
			progressBar1.Margin = new Padding(6);
			progressBar1.Maximum = 1000;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(418, 31);
			progressBar1.TabIndex = 1;

			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			button1.Location = new Point(26, 22);
			button1.Margin = new Padding(6);
			button1.Name = "button1";
			button1.Size = new Size(182, 42);
			button1.TabIndex = 0;
			button1.Text = "Start Calculation";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(button1_Click);

			progressBar1.Location = new Point(26, 76);
			progressBar1.Margin = new Padding(6);
			progressBar1.Maximum = 1000;
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(418, 31);
			progressBar1.TabIndex = 1;

			label1.AutoSize = true;
			label1.Location = new Point(460, 78);
			label1.Margin = new Padding(6, 0, 6, 0);
			label1.Name = "label1";
			label1.Size = new Size(41, 25);
			label1.TabIndex = 2;
			label1.Text = "0%";

			this.AutoScaleDimensions = new SizeF(11F, 24F);
			this.AutoScaleMode = AutoScaleMode.Font;
			// this.Size = new System.Drawing.Size(274, 27);
			this.ClientSize = new Size(549, 133);
			this.Controls.Add(label1);
			this.Controls.Add(progressBar1);
			this.Controls.Add(button1);
			((System.ComponentModel.ISupportInitialize)(timer1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(timer2)).EndInit();


			this.Name = "UserControl1";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private ProgressBar progressBar1;
		private Label label1;
		private Button button1;
	}
	public class Data{
		public DateTime TimeStamp { get; set; }
		public float Value { get; set; }
		public override string ToString() {
			return string.Format("TimeStamp={0} {1}, Value={2}", TimeStamp.ToLongDateString(), TimeStamp.ToLongTimeString(), Value);
		}
	}
	public class CircularBuffer<T> : IEnumerable<T> {
		internal T[] _array;
		internal Int32 _start;
		internal Int32 _end;
		private Int32 _size;
		private Boolean _isDynamic;
		private Boolean _isInfinite;

		public CircularBuffer() : this(0, true, false) {
		}

		public CircularBuffer(Int32 size) : this(size, true, false) {
		}

		public CircularBuffer(Int32 size, Boolean isInfinite, Boolean isDynamic) {
			_array = new T[size];
			_start = 0;
			_end = 0;
			_size = 0;
			_isDynamic = isDynamic;
			_isInfinite = isInfinite;
		}

		public void AddFirst(T data)  {
			if (!IsFull || !_isInfinite) {
				if (_size != 0) {
					_start = GetPreviousPosition(_start);
					if (_start == _end) {
						_end = GetNextPosition(_end);
					} else {
						_size++;
					}
				} else {
					_start = 0;
					_size++;
				}
				_array[_start] = data;
			} else if (_isDynamic) {
				Extend();
				AddFirst(data);
			} else {
				throw new Exception("Array is Full");
			}
		}

		public void AddLast(T data) {
			if (!IsFull || _isInfinite) {
				if (_size != 0) {
					_end = GetNextPosition(_end);
					if (_start == _end) {
						_start = GetNextPosition(_start);
					} else {
						_size++;
					}
				} else {
					_end = 0;
					_size++;
				}
				_array[_end] = data;
			} else if (_isDynamic) {
				Extend();
				AddLast(data);
			} else {
				throw new Exception("Array is Full");
			}
		}

		public void RemoveFirst() {
			if (!IsEmpty) {
				_start = GetNextPosition(_start);
				_size--;
			} else {
				throw new Exception("Array is Empty");
			}
		}

		public void RemoveLast() {
			if (!IsEmpty) {
				_end = GetPreviousPosition(_end);
				_size--;
			} else {
				throw new Exception("Array is Empty");
			}
		}

		private void Extend() {
			// The name 'Console' does not exist in the current context (CS0103)
			// Console.Error.WriteLine("Extening to Capacity: " + Capacity * 2 );
			Extend(Capacity * 2);
		}

		private void Extend(Int32 capacity) {
			T[] extendedArray = new T[capacity];
			if (capacity < Capacity) {
				Int32 end = 0;
				Int32 size = 0;
				for (Int32 i = 0; i < extendedArray.Length && i < _array.Length; i++, end = GetNextPosition(end), size++) {
					extendedArray[i] = _array[end];
				}
				_start = 0;
				_end = --end;
				_size = size;

			} else {
				_array.CopyTo(extendedArray, 0);
			}
			_array = extendedArray;
		}

		private Int32 GetNextPosition(Int32 pos) {

			if (pos == Capacity - 1) {
				return 0;
			} else {
				return pos + 1;
			}
		}

		private Int32 GetPreviousPosition(Int32 pos) {
			if (pos == 0) {
				return Capacity - 1;
			} else {
				return pos - 1;
			}
		}

		IEnumerator IEnumerable.GetEnumerator(){
		    return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator(){
		    for (int i = 0; i < _size; i++){
        		yield return this[i];
    		}
		}
		public T this[Int32 index] {
			get {
				if (index >= 0 && index < _size) {
					Int32 iBuffer = _start;
					for (Int32 incremented = 0; incremented < index; incremented++) {
						iBuffer = GetNextPosition(iBuffer);
					}
					return _array[iBuffer];
				} else {
					throw new Exception("Invalid Index");
				}
			}
			set {
				if (index >= 0 && index <= _size) {
					Int32 iBuffer = _start;
					for (Int32 incremented = 0; incremented < index; incremented++) {
						iBuffer++;
					}
					_array[iBuffer] = value;
				} else {
					throw new Exception("Invalid Index");
				}
			}
		}

		public T[] ToArray() {
			Int32 numOfElementsProcessed = 0;
			T[] array = new T[_size];
			Int32 iOutputArray = 0;
			for (Int32 iBuffer = _start; numOfElementsProcessed < _size; iBuffer = GetNextPosition(iBuffer), numOfElementsProcessed++, iOutputArray++) {
				array[iOutputArray] = _array[iBuffer];
			}
			return array;
		}

		public List<T> ToList() {
			Int32 numOfElementsProcessed = 0;
			var list = new List<T>();
			for (Int32 iBuffer = _start; numOfElementsProcessed < _size; iBuffer = GetNextPosition(iBuffer), numOfElementsProcessed++) {
				list.Add(_array[iBuffer]);
			}
			return list;
		}

		protected Int32 Start {
			set {
				if (value >= 0 && value < _array.Length) {
					_start = value;
				} else {
					throw new Exception("Inappropriate Index");
				}
			}
			get {
				return _start;
			}
		}

		protected Int32 End {
			set {
				if (value >= 0 && value < _array.Length) {
					_end = value;
				} else {
					throw new Exception("Inappropriate Index");
				}
			}
			get {
				return _end;
			}
		}

		public Int32 Size {
			get {
				return _size;
			}
		}

		public Int32 Capacity {
			set {
				Extend(value);
			}
			get {
				return _array.Length;
			}
		}

		public Boolean IsDynamic {
			set {
				_isDynamic = value;
			}
			get {
				return _isDynamic;
			}
		}

		public Boolean IsInfinite {
			set {
				_isInfinite = value;
			}
			get {
				return _isInfinite;
			}
		}

		private Boolean IsEmpty {
			get {
				return _size == 0;
			}
		}

		private Boolean IsFull {
			get {
				return _size == Capacity;
			}
		}
	}
	public static class ExtensionMethod {
		public static TResult safeInvoke<T, TResult>(this T iSynchronizeInvoke, Func<T, TResult> call) where T : ISynchronizeInvoke  {
			if (iSynchronizeInvoke.InvokeRequired) {
				IAsyncResult result = iSynchronizeInvoke.BeginInvoke(call, new object[] { iSynchronizeInvoke });
				object endResult = iSynchronizeInvoke.EndInvoke(result);
				return (TResult)endResult;
			} else
				return call(iSynchronizeInvoke);
		}

		public static void safeInvoke<T>(this T iSynchronizeInvoke, Action<T> call) where T : ISynchronizeInvoke {
			if (iSynchronizeInvoke.InvokeRequired)
				iSynchronizeInvoke.BeginInvoke(call, new object[] { iSynchronizeInvoke });
			else
				call(iSynchronizeInvoke);
		}
	}

}


"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll', 'System.Collections.dll', 'System.xml.dll','System.Xml.Linq.dll'


<#
function get_process_id_by_commandline {
  param (
    $name = $null,
    $value = $null
 )
  [string]$result = $null
  write-host ('name: "{0}" value: "{1}"' -f $name, $value)
  [string]$filter="Name = '${name}'"
  write-debug ('filter: {0}' -f $filter )
  $data = get-WmiObject -Class Win32_Process -filter $filter |
  select-object -property Name,ProcessId,CommandLine
  write-debug ('{0} rows' -f $data.Count)
  if ($data.Count -gt 0 ) {
    0..($data.Count-1) | foreach-object {
      $cnt = $_

      if ($data[$cnt].CommandLine -match $value) {
        $result = $data[$cnt].ProcessId
        write-debug ('CommanLine matched "{0}"' -f $value )
        write-debug ('name: {0} processid: {1}' -f $data[$cnt].Name, $data[$cnt].ProcessId ) | format-list
      } else {
        write-debug ('CommanLine NOT matched "{0}"' -f $value )
        write-debug $data[$cnt].CommandLine
      }
    }
  }
  $result
}


function get_performance_counter_instance {
  param (
    [int] $processid = -1,
    [string] $name = 'chrome'
 )

  [System.Diagnostics.PerformanceCounterCategory] $performanceCounterCategory = new-object System.Diagnostics.PerformanceCounterCategory -argumentlist ([string]'Process')
  $performanceCounterCategory.GetInstanceNames() | foreach-object {
    $instanceName = $_
    if (-not ($instanceName -match $name)) {
      return
    }
    write-debug ('instanceName: {0}' -f $instanceName)
    $performanceCounter = new-object System.Diagnostics.PerformanceCounter('Process', 'ID Process', $instanceName, $true)
    $rawValue  = $performanceCounter.RawValue
    write-debug ('rawValue: {0}' -f $rawValue)
    if ($rawValue -eq $processid) {
      write-output $instanceName
      # optional - exit for-each
    }
  }
}


$processid = get_process_id_by_commandline -name $name -value $value
write-output ('processid: {0}' -f $processid)
# NOTE:
$name = 'java'
$instance = get_performance_counter_instance -processid $processid -name $name
write-output ('peformance counter instance: {0}' -f $instance )
#>

@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }

$f = new-object System.Windows.Forms.Form
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.BackColor = [System.Drawing.Color]::LightGray
$f.SuspendLayout()

if ($instance -ne '') {
  write-output ('\{0}({1})\{2}' -f $category, $instance, $counter)
} else {
  write-output ('\{0}\{1}' -f $category, $counter)
}

$cbc1 = new-object Utils.UserControl1

$cbc1.CategoryName = $category
$cbc1.CounterName = $counter
$cbc1.InstanceName = $instance
$cbc1.AverageInterval = 30000
$cbc1.CollectInterval = 1000
$cbc1.Capacity = 900
$cbc1.Debug = $true
$cbc1.Rounds = 1000


$cbc1.BackColor = [System.Drawing.Color]::Transparent
$cbc1.Location = new-object System.Drawing.Point (0,0)
$cbc1.AutoScaleDimensions = new-object System.Drawing.Size (11,24)
$cbc1.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$cbc1.Name = 'UserControl1'
$cbc1.ClientSize = new-object System.Drawing.Size (549, 133)
$cbc1.TabIndex = 1

$f.Controls.Add($cbc1)
$f.AutoScaleDimensions = new-object System.Drawing.Size(11, 24)
$f.ClientSize = new-object System.Drawing.Size(549, 133)
			
$f.Name = 'Form1'

$f.Text = 'Progress Control'
$f.ResumeLayout($false)

[void]$f.ShowDialog()

