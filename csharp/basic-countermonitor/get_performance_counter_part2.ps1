#Copyright (c) 2015,2026 Serguei Kouzmine
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
  [int] $processid = 42068,
  [string] $value = 'example.Appication',
  [string] $name = 'java.exe',
  [switch]$debug # currently unused
)

[bool]$debug_flag = [bool]$PSBoundParameters['debug'].IsPresent -bor $DebugPreference -eq 'Continue'
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

using System.Timers;
using System.Windows.Forms;
using System;

namespace Utils
{
    public partial class CircularProgressControl : UserControl
    {

        private string result;
		public string Result { get { 
			Console.Error.WriteLine(String.Format("result: {0}", this.result));
			return result;}
		}
		private CircularBuffer<Data> buffer;

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

		private int averageInterval = 30000;
		private int collectInterval = 1000;
		private static int capacity = 900;
		// NOTE: the default value os
		// categoryNAme, counterName and instanceName about to be overwrittedn my app config values
		private string categoryName = "Memory";
		private string counterName = "Available Bytes";
		private string instanceName = "";

        private const int DEFAULT_INTERVAL = 60;
        private readonly Color DEFAULT_TICK_COLOR = Color.FromArgb(58, 58, 58);
        private const int DEFAULT_TICK_WIDTH = 2;
        private const int MINIMUM_INNER_RADIUS = 4;
        private const int MINIMUM_OUTER_RADIUS = 8;
        private Size MINIMUM_CONTROL_SIZE = new Size(28, 28);
        private const int MINIMUM_PEN_WIDTH = 2;


        public enum Direction
        {
            CLOCKWISE,
            ANTICLOCKWISE
        }


        private int m_Interval;
        Pen m_Pen = null;
        PointF m_CentrePt = new PointF();
        int m_InnerRadius = 0;
        int m_OuterRadius = 0;
        int m_StartAngle = 0;
        int m_AlphaStartValue = 0;
        int m_SpokesCount = 0;
        int m_AngleIncrement = 0;
        int m_AlphaDecrement = 0;
        System.Windows.Forms.Timer m_Timer = null;

        public int Interval
        {
            get
            {
                return m_Interval;
            }
            set
            {
                if (value > 0)
                {
                    m_Interval = value;
                }
                else
                {
                    m_Interval = DEFAULT_INTERVAL;
                }
            }
        }
        public Color TickColor { get; set; }
        public Direction Rotation { get; set; }
        private bool m_clockwise;
        public bool Clockwise 
        {
            get
            {
                return m_clockwise;
            }
            set
            {
                m_clockwise = value;
                if (m_clockwise){ 
                   this.Rotation = Direction.CLOCKWISE;
                } else { 
                   this.Rotation = Direction.ANTICLOCKWISE;
                 }
            }
        }

        public int StartAngle
        {
            get
            {
                return m_StartAngle;
            }
            set
            {
                m_StartAngle = value;
            }
        }

        public CircularProgressControl()
        {
            this.DoubleBuffered = true;

            
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Color.Transparent;
            this.TickColor = DEFAULT_TICK_COLOR;
            this.MinimumSize = MINIMUM_CONTROL_SIZE;
            this.Interval = DEFAULT_INTERVAL;
            this.Rotation = Direction.CLOCKWISE;
            this.StartAngle = 270;
            m_SpokesCount = 12;
            m_AlphaStartValue = 255;
            m_AngleIncrement = (int)(360/m_SpokesCount);
            m_AlphaDecrement = (int)((m_AlphaStartValue - 15) / m_SpokesCount);

            m_Pen = new Pen(TickColor, DEFAULT_TICK_WIDTH);
            m_Pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            m_Pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            m_Timer = new System.Windows.Forms.Timer();
            m_Timer.Interval = this.Interval;
            m_Timer.Tick += new EventHandler(OnTimerTick);
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            if (Rotation == Direction.CLOCKWISE)
            {
                m_StartAngle += m_AngleIncrement;

                if (m_StartAngle >= 360)
                    m_StartAngle = 0;
            }
            else if (Rotation == Direction.ANTICLOCKWISE)
            {
                m_StartAngle -= m_AngleIncrement;

                if (m_StartAngle <= -360)
                    m_StartAngle = 0;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // All the paintin will be handled by us.
            //base.OnPaint(e);

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Since the Rendering of the spokes is dependent upon the current size of the 
            // control, the following calculation needs to be done within the Paint eventhandler.
            int alpha = m_AlphaStartValue;
            int angle = m_StartAngle;
            // Calculate the location around which the spokes will be drawn
            int width = (this.Width < this.Height) ? this.Width : this.Height;
            m_CentrePt = new PointF(this.Width / 2, this.Height / 2);
            // Calculate the width of the pen which will be used to draw the spokes
            m_Pen.Width = (int)(width / 15);
            if (m_Pen.Width < MINIMUM_PEN_WIDTH)
                m_Pen.Width = MINIMUM_PEN_WIDTH;
            // Calculate the inner and outer radii of the control. The radii should not be less than the
            // Minimum values
            m_InnerRadius = (int)(width * (140 / (float)800));
            if (m_InnerRadius < MINIMUM_INNER_RADIUS)
                m_InnerRadius = MINIMUM_INNER_RADIUS;
            m_OuterRadius = (int)(width * (250 / (float)800));
            if (m_OuterRadius < MINIMUM_OUTER_RADIUS)
                m_OuterRadius = MINIMUM_OUTER_RADIUS;

            // Render the spokes
            for (int i = 0; i < m_SpokesCount; i++)
            {
                PointF pt1 = new PointF(m_InnerRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), m_InnerRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));
                PointF pt2 = new PointF(m_OuterRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), m_OuterRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));

                pt1.X += m_CentrePt.X;
                pt1.Y += m_CentrePt.Y;
                pt2.X += m_CentrePt.X;
                pt2.Y += m_CentrePt.Y;
                m_Pen.Color = Color.FromArgb(alpha, this.TickColor);
                e.Graphics.DrawLine(m_Pen, pt1, pt2);

                if (Rotation == Direction.CLOCKWISE)
                {
                    angle -= m_AngleIncrement;
                }
                else if (Rotation == Direction.ANTICLOCKWISE)
                {
                    angle += m_AngleIncrement;
                }

                //if (i < 5)
                //    alpha -= 45;
                alpha -= m_AlphaDecrement;
            }
        }

        private double ConvertDegreesToRadians(int degrees)
        {
            return ((Math.PI / (double)180) * degrees);
        }

        public void Start()
        {
        				
			timer1 = new System.Timers.Timer();
			timer2 = new System.Timers.Timer();

        	buffer = new CircularBuffer<Data>(capacity);

            if (m_Timer != null)
            {
                m_Timer.Interval = this.Interval;
                m_Timer.Enabled = true;
            }
            // planted the code responsible for metric collection
            
				timer1.Interval = collectInterval;
				timer1.Enabled = true;
				timer1.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => CollectMetrics());
				timer1.Start();

				timer2.Interval = averageInterval;
				timer2.Elapsed += new ElapsedEventHandler((object source, ElapsedEventArgs args) => Commit());
				timer2.Enabled = true;
				timer2.Start();
	
            
        }

        public void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
            }
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
				performanceCounter.InstanceName = instanceName == "" ? null : instanceName;
				// value = (long)performanceCounter.RawValue;
				value = performanceCounter.NextValue();
			} catch (InvalidOperationException e) {
				Console.Error.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()));
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
				Console.Error.WriteLine(this.result);

			} catch (Exception e) {
				// System.InvalidOperationException: Sequence contains no elements
				Console.Error.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
		}
   }
	public class Data{
		public DateTime TimeStamp { get; set; }
		public float Value { get; set; }
		public override string ToString() {
			return string.Format("TimeStamp={0} {1}, Value={2}", TimeStamp.ToLongDateString(), TimeStamp.ToLongTimeString(), Value);
		}
	}

	public class CircularBuffer<T> : IEnumerator<T>
	{
		internal T[] _array;
		internal Int32 _start;
		internal Int32 _end;
		private Int32 _size;
		private Boolean _isDynamic;
		private Boolean _isInfinite;
		private Int32 _currentPosition = -1;
		private Int32 _numOfElementsProcessed = 0;
		public CircularBuffer()
			: this(0, true, false)
		{
		}

		public CircularBuffer(Int32 size)
			: this(size, true, false)
		{
		}

		public CircularBuffer(Int32 size, Boolean isInfinite, Boolean isDynamic)
		{
			_array = new T[size];
			_start = 0;
			_end = 0;
			_size = 0;
			_isDynamic = isDynamic;
			_isInfinite = isInfinite;
		}

		public void AddFirst(T data)
		{
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

		public void AddLast(T data)
		{
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

		public void RemoveFirst()
		{
			if (!IsEmpty) {
				_start = GetNextPosition(_start);
				_size--;
			} else {
				throw new Exception("Array is Empty");
			}
		}

		public void RemoveLast()
		{
			if (!IsEmpty) {
				_end = GetPreviousPosition(_end);
				_size--;
			} else {
				throw new Exception("Array is Empty");
			}
		}

		private void Extend()
		{
			// The name 'Console' does not exist in the current context (CS0103)
			// Console.WriteLine("Extening to Capacity: " + Capacity * 2 );
			Extend(Capacity * 2);
		}

		private void Extend(Int32 capacity)
		{
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

		private Int32 GetNextPosition(Int32 pos)
		{

			if (pos == Capacity - 1) {
				return 0;
			} else {
				return pos + 1;
			}
		}

		private Int32 GetPreviousPosition(Int32 pos)
		{
			if (pos == 0) {
				return Capacity - 1;
			} else {
				return pos - 1;
			}
		}

		void IDisposable.Dispose()
		{
			_currentPosition = -1;
			_numOfElementsProcessed = 0;
		}

		public bool MoveNext()
		{
			if (_currentPosition == -1) {
				_currentPosition = _start;
				_numOfElementsProcessed++;
			} else {
				_currentPosition = GetNextPosition(_currentPosition);
				_numOfElementsProcessed++;
			}
			return (_numOfElementsProcessed <= Size);
		}

		public void Reset()
		{
			_start = 0;
			_end = 0;
			_size = 0;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this;
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

		public T[] ToArray()
		{
			Int32 numOfElementsProcessed = 0;
			T[] array = new T[_size];
			Int32 iOutputArray = 0;
			for (Int32 iBuffer = _start; numOfElementsProcessed < _size; iBuffer = GetNextPosition(iBuffer), numOfElementsProcessed++, iOutputArray++) {
				array[iOutputArray] = _array[iBuffer];
			}
			return array;
		}

		public List<T> ToList()
		{
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

		object IEnumerator.Current {
			get {
				return Current;
			}
		}

		public T Current {
			get {
				try {
					return _array[_currentPosition];
				} catch (IndexOutOfRangeException) {
					throw new InvalidOperationException();
				}
			}
		}
	}	
}


"@ -ReferencedAssemblies 'System.Windows.Forms.dll','System.Drawing.dll','System.Data.dll', 'System.Collections.dll', 'System.xml.dll','System.Xml.Linq.dll'



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
       write-debug $data[$cnt].CommandLine

       if ($data[$cnt].CommandLine -match 'example.Application') {
         $result = $data[$cnt].ProcessId
         write-debug ('CommanLine matched "{0}"' -f $value )
         write-debug ('name: {0} processid: {1}' -f $data[$cnt].Name, $data[$cnt].ProcessId ) | format-list
       }

    }
  }
  $result
}


function get_performance_counter {
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



@( 'System.Drawing','System.Windows.Forms') | ForEach-Object { [void][System.Reflection.Assembly]::LoadWithPartialName($_) }
$f = New-Object System.Windows.Forms.Form
$f.AutoScaleDimensions = New-Object System.Drawing.SizeF (6.0,13.0)
$f.AutoScaleMode = [System.Windows.Forms.AutoScaleMode]::Font
$f.BackColor = [System.Drawing.Color]::LightGray
$f.ClientSize = New-Object System.Drawing.Size (257,119)

$button1 = New-Object System.Windows.Forms.Button
$label = New-Object System.Windows.Forms.Label
$cbc1 = New-Object Utils.CircularProgressControl
$f.SuspendLayout()

$label.AutoSize = $true
$label.Location = new-object System.Drawing.Point(20, 78)
$label.Margin = new-object System.Windows.Forms.Padding(6, 0, 6, 0)
$label.Name = "label1"
$label.Size = new-object System.Drawing.Size(224, 27)
$label.TabIndex = 2

$button1.Location = New-Object System.Drawing.Point (170,40)
$button1.Name = 'button1'
$button1.Size = New-Object System.Drawing.Size (75,23)
$button1.TabIndex = 0
$button1.Text = 'Start'
$button1.UseVisualStyleBackColor = true
$button1.add_click.Invoke({
    param(
      [object]$sender,
      [System.EventArgs]$eventargs
    )
    if ($button1.Text -eq "Start")
    {
      $button1.Text = 'Stop'
      $cbc1.Start()
    }
    else
    {
      $button1.Text = 'Start'
      $cbc1.Stop()
    }
  })


$cbc1.BackColor = [System.Drawing.Color]::Transparent
$cbc1.Interval = 60
$cbc1.Location = New-Object System.Drawing.Point (10,20)
$cbc1.MinimumSize = New-Object System.Drawing.Size (56,56)
$cbc1.Name = "circularProgressControl1"
$cbc1.Clockwise = $true
$cbc1.Size = New-Object System.Drawing.Size (56,56)
$cbc1.StartAngle = 270
$cbc1.TabIndex = 1
$cbc1.TickColor = [System.Drawing.Color]::DarkBlue

$f.Controls.Add($label)
$f.Controls.Add($button1)
$f.Controls.Add($cbc1)
$f.Name = 'Form1'
$f.Text = 'Progress Control'
$f.ResumeLayout($false)

[void]$f.ShowDialog()

