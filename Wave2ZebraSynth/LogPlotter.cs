// reference http://www.computer-consulting.com/logplotter.htm

using System.Drawing; //holds graphics, bitmap, brush, etc
using System.Drawing.Drawing2D; //holds smoothingmode, pentype enumerations
using System.Drawing.Imaging; //holds imageformat enumeration
using System; //holds log function
using System.IO; //holds file class for deleting existing files

namespace Wave2ZebraSynth
{
	public class LogPlotter
	{

		public LogPlotter()
		{
			_yRangeEnd = 90;
			_yGrid = 10;
		}

		public LogPlotter(bool alternateRange)
		{
			_yRangeEnd = 260;
			_yGrid = 20;
		}

		#region  Private Declarations

		private int x0;
		private int x1;
		private int y0;
		private int y1;
		private int w0;
		private int w1;
		private int h0;
		private int h1;
		private int x;
		private int y;
		private int n;
		private int d;
		private int d1;
		private string s;

		#endregion

		#region  Public Properties

		private Color _colorDraw = Color.DarkBlue;
		public Color ColorDraw
		{
			get
			{
				return _colorDraw;
			}
			set
			{
				_colorDraw = value;
			}
		}

		private Color _colorGrid = Color.LightGray;
		public Color ColorGrid
		{
			get
			{
				return _colorGrid;
			}
			set
			{
				_colorGrid = value;
			}
		}

		private Color _colorBg = Color.White;
		public Color ColorBg
		{
			get
			{
				return _colorBg;
			}
			set
			{
				_colorBg = value;
			}
		}

		private Color _colorAxis = Color.Black;
		public Color ColorAxis
		{
			get
			{
				return _colorAxis;
			}
			set
			{
				_colorAxis = value;
			}
		}

		private Font _fontAxis = new Font("Arial", 8);
		public Font FontAxis
		{
			get
			{
				return _fontAxis;
			}
			set
			{
				_fontAxis = value;
			}
		}

		private int _penWidth = 2;
		public int PenWidth
		{
			get
			{
				return _penWidth;
			}
			set
			{
				_penWidth = value;
			}
		}

		private int _borderTop = 30;
		public int BorderTop
		{
			get
			{
				return _borderTop;
			}
			set
			{
				_borderTop = value;
			}
		}

		private int _borderLeft = 50;
		public int BorderLeft
		{
			get
			{
				return _borderLeft;
			}
			set
			{
				_borderLeft = value;
			}
		}

		private int _borderBottom = 30;
		public int BorderBottom
		{
			get
			{
				return _borderBottom;
			}
			set
			{
				_borderBottom = value;
			}
		}

		private int _borderRight = 30;
		public int BorderRight
		{
			get
			{
				return _borderRight;
			}
			set
			{
				_borderRight = value;
			}
		}

		private double _xRangeStart = 0.01;
		public double XRangeStart
		{
			get
			{
				return _xRangeStart;
			}
			set
			{
				_xRangeStart = value;
			}
		}

		private double _xRangeEnd = 10000000;
		public double XRangeEnd
		{
			get
			{
				return _xRangeEnd;
			}
			set
			{
				_xRangeEnd = value;
			}
		}

		private double _yRangeStart = 0;
		public double YRangeStart
		{
			get
			{
				return _yRangeStart;
			}
			set
			{
				_yRangeStart = value;
			}
		}

		private double _yRangeEnd;
		public double YRangeEnd
		{
			get
			{
				return _yRangeEnd;
			}
			set
			{
				_yRangeEnd = value;
			}
		}

		private int _xGrid = 100;
		public int XGrid
		{
			get
			{
				return _xGrid;
			}
			set
			{
				_xGrid = value;
			}
		}

		private int _yGrid;
		public int YGrid
		{
			get
			{
				return _yGrid;
			}
			set
			{
				_yGrid = value;
			}
		}

		private int _xLogBase = 10;
		public int XLogBase
		{
			get
			{
				return _xLogBase;
			}
			set
			{
				_xLogBase = value;
			}
		}

		private int _yLogBase = 0;
		public int YLogBase
		{
			get
			{
				return _yLogBase;
			}
			set
			{
				_yLogBase = value;
			}
		}

		#endregion

		public void Render(double[] xData, double[] yData, string filename)
		{
			Bitmap outputBitmap = new Bitmap(600, 300);
			Graphics g = Graphics.FromImage(outputBitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			RectangleF clientRectangle = new RectangleF(0F, 0F, 600F, 300F);
			x0 = (int) clientRectangle.Left + BorderLeft;
			y0 = (int) clientRectangle.Top + BorderTop;
			w0 = (int) clientRectangle.Width - BorderLeft - BorderRight;
			h0 = (int) clientRectangle.Height - BorderTop - BorderBottom;
			x1 = (int) clientRectangle.Right - BorderRight;
			y1 = (int) clientRectangle.Bottom - BorderBottom;
			g.FillRectangle(new SolidBrush(ColorBg), clientRectangle);
			this.DrawVerticalLines(g);
			w1 = d * n;
			this.DrawHorizontalLines(g);
			Pen penAxis = new Pen(ColorAxis, 1F);
			h1 = d * n;
			g.DrawRectangle(penAxis, x0, y0, w0, h0); // draw axis
			h0 = h1; //must correct internal width & height since equidistant
			w0 = w1; //gridlines may not fit in axis rectangle w/o rounding errors
			this.DrawData(g, xData, yData);
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}
			outputBitmap.Save(filename, ImageFormat.Jpeg);
			outputBitmap.Dispose();
			g.Dispose();
		}

		private void DrawVerticalLines(Graphics g)
		{
			Pen penGrid = new Pen(ColorGrid, 1F);
			SolidBrush brushAxis = new SolidBrush(ColorAxis);
			n = Convert.ToInt32(Math.Log(XRangeEnd, XLogBase) - Math.Log(XRangeStart, XLogBase)); //get the x width converted to log10
			if (n == 0)
			{
				n = 1;
			}
			d = w0 / n;
			for (int i = 0; i <= n; i++)
			{
				x = x0 + i * d;
				if (i < n)
				{
					for (int j = 1; j < XLogBase; j++)
					{
						d1 = Convert.ToInt32(Math.Log(j, XLogBase) * d);
						g.DrawLine(penGrid, x + d1, y0, x + d1, y1);
					}
				}
				s = this.LargeFormat(Convert.ToString(Math.Pow(XLogBase, Math.Log(XRangeStart, XLogBase) + i)));
				SizeF sf = g.MeasureString(s, FontAxis);
				g.DrawString(s, FontAxis, brushAxis, (float)(x - sf.Width / 2.0), (float)(y1 + sf.Height / 2.0));
			}
		}

		private string LargeFormat(string value)
		{
			string result = null;
			switch (value)
			{
				case "1000":
					result = "1K";
					break;
				case "10000":
					result = "10K";
					break;
				case "100000":
					result = "100K";
					break;
				case "1000000":
					result = "1M";
					break;
				case "10000000":
					result = "10M";
					break;
				default:
					result = value;
					break;
			}
			return result;
		}

		private void DrawHorizontalLines(Graphics g)
		{
			Pen penGrid = new Pen(ColorGrid, 1F);
			SolidBrush brushAxis = new SolidBrush(ColorAxis);
			n = Convert.ToInt32((YRangeEnd - YRangeStart) / YGrid);
			if (n == 0)
			{
				n = 1;
			}
			d = h0 / n;
			for (int i = 0; i <= n; i++)
			{
				y = y1 - i * d;
				g.DrawLine(penGrid, x0, y, x1, y);
				string s = Convert.ToString(YRangeStart + (YRangeEnd - YRangeStart) * i / (double)n);
				SizeF sf = g.MeasureString(s, FontAxis);
				g.DrawString(s, FontAxis, brushAxis, (float)(x0 - sf.Width - sf.Height / 4.0), (float)(y - sf.Height / 2.0));
			}
		}

		private void DrawData(Graphics g, double[] xdata, double[] ydata)
		{
			Pen penDraw = new Pen(ColorDraw, (float)(PenWidth));
			Point[] pts = new Point[xdata.Length];
			Point lastValidPt = new Point(x0, y1);
			for (int i = 0; i < pts.Length; i++) //convert points to fit inside graphing area
			{
				try
				{
					pts[i].X = Convert.ToInt32(x0 + (Math.Log(xdata[i], XLogBase) - Math.Log(XRangeStart, XLogBase)) / (Math.Log(XRangeEnd, XLogBase) - Math.Log(XRangeStart, XLogBase)) * w0);
					pts[i].Y = Convert.ToInt32(y1 - (ydata[i] - YRangeStart) / (YRangeEnd - YRangeStart) * h0);
					lastValidPt = pts[i];
				}
				catch (Exception ex)
				{
					pts[i] = lastValidPt; //redraw last valid point on error
				}
			}
			for (int i = 0; i < pts.Length; i++) //now draw the points
			{
				if (i > 0)
				{
					g.DrawLine(penDraw, pts[i - 1], pts[i]);
				}
			}
		}
	}
}
