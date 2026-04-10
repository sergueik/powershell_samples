using System.Collections.Generic;
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
			Debug.WriteLine(String.Format("result: {0}", this.result));
			return result;}
		}
		private CircularBuffer<Data> buffer;

		private System.Timers.Timer timer1;
		private System.Timers.Timer timer2;

		private Boolean debug;
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
				Debug.WriteLine(String.Format("Exception reading \"{0}\\{1}\\{2}\": {3}", categoryName, counterName, "0", e.ToString()));
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
				Debug.WriteLine(this.result);

			} catch (Exception e) {
				// System.InvalidOperationException: Sequence contains no elements
				Debug.WriteLine(String.Format("Exception: {0}", e.ToString()));
			}
		}
   }
}

