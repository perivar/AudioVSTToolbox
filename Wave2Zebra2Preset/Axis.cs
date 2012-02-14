using System;
using System.Collections;
using System.Drawing;

namespace Wave2Zebra2Preset
{
	
	///
	/// <summary> * includes a static function for selecting and labeling graph axis tic labels.
	/// * given a numeric range and a maximum number of tics,
	/// * this class can produce a list of labels with the nicest round numbers
	/// * not exceeding a given maximum number of labels.
	/// * the label generation code was extracted from the public domain
	/// * <a href="http://ptolemy.eecs.berkeley.edu/">Ptolomy project</a>
	/// * at UC Berkeley, taken from ptolemy/plot/PlotBox.java.
	/// *
	/// * i added another static method to compute and draw an axis into
	/// * a given AWT Graphics object. i extracted the code for producing linear
	/// * labels and threw out the vast majority of code that attempted to produce
	/// * log scale labels since that code was very broken. it was noted int the
	/// * Ptolomy code that the log label generation was itself based on
	/// * a file named xgraph.c by David Harrisonmy, and the comments say that
	/// * the original code breaks down in certain cases. my drawAxis method
	/// * can still draw nicely labeling log scale axes because i simply use
	/// * the linear scale label generation code from Ptolomy and plot the tics in
	/// * their proper locations on a log scale. the resulting code produced exactly
	/// * the same results as the Ptolemy code for log scales in those ranges where
	/// * the Ptolemy code did work, so this design is much better in all cases and
	/// * uses only a fraction of the original complexity. still, it can probably
	/// * be further improved though the exact problem is not well defined.
	/// * 
	/// * @author Melinda Green </summary>
	/// 
	public class Axis
	{
		private static bool DEBUG = false;
		public const int X_AXIS = 0, Y_AXIS = 1;
		
		// For use in calculating log base 10. A log times this is a log base 10.
		private static double LOG10SCALE = 1 / Math.Log(10);
		
		// handy static methods
		public static double log10(double val)
		{
			return Math.Log(val) * LOG10SCALE;
		}
		public static double exp10(double val)
		{
			return Math.Exp(val / LOG10SCALE);
		}
		public static float flog10(double val)
		{
			return (float)log10(val);
		}
		public static float fexp10(double val)
		{
			return (float)exp10(val);
		}


		/// <summary> * this is the central method of this class.
		/// * takes axis range parameters and produces a list of string
		/// * representations of nicely rounded numbers within the given range.
		/// * these strings are intended for use as axis tic labels.
		/// * note: to find out where to plot each tic label simply
		/// * use <br><code>float ticval = Float.parseFloat(ticstring);</code> </summary>
		/// * <param name="ticMinVal"> no tics will be created for less than this value. </param>
		/// * <param name="ticMaxVal"> no tics will be created for greater than this value. </param>
		/// * <param name="maxTics"> returned vector will contain no more labels than this number. </param>
		/// * <returns> a Vector containing formatted label strings which should also
		/// * be parsable into floating point numbers (in order to plot them). </returns>
		///
		public static ArrayList computeTicks(double ticMinVal, double ticMaxVal, int maxTicks)
		{
			double xStep = roundUp((ticMaxVal-ticMinVal)/maxTicks);
			int numfracdigits = numFracDigits(xStep);

			// Compute x starting point so it is a multiple of xStep.
			double xStart = xStep*Math.Ceiling(ticMinVal/xStep);
			ArrayList labels = new ArrayList();
			// Label the axis. The labels are quantized so that
			// they don't have excess resolution.
			for (double xpos=xStart; xpos<=ticMaxVal; xpos+=xStep)
			{
				labels.Add(formatNum(xpos, numfracdigits));
			}
			return labels;
		}

		/// <summary> * high-level method for drawing a chart axis line plus labeled tic marks.
		/// * introduces a dependancy on AWT because it takes a Graphics parameter.
		/// * perhaps this method belongs in some higher-level class but i added it
		/// * here since it's highly related with the tic lable generation code.
		/// * 
		/// * @author Melinda Green
		/// * </summary>
		/// * <param name="axis"> is one of Axis.X_AXIS or Axis.Y_AXIS. </param>
		/// * <param name="maxTics"> is the maximum number of labeled tics to draw.
		/// * note: the actual number drawn may be less. </param>
		/// * <param name="lowVal"> is the smallest value tic mark that may be drawn.
		/// * note: the lowest valued tic label may be greater than this limit. </param>
		/// * <param name="highVal"> is the largest value tic mark that may be drawn.
		/// * note: the highest valued tic label may be less than this limit. </param>
		/// * <param name="screenStart"> is the coordinate in the low valued direction. </param>
		/// * <param name="screenEnd"> is the coordinate in the high valued direction. </param>
		/// * <param name="offset"> is the coordinate in the direction perpendicular to
		/// * the specified direction. </param>
		/// * <param name="logScale"> is true if a log scale axis is to be drawn,
		/// * false for a linear scale. </param>
		/// * <param name="screenHeight"> is needed to flip Y coordinates. </param>
		/// * <param name="g"> is the AWT Graphics object to draw into.
		/// * note: all drawing will be done in the current color of the given
		/// * Graphics object. </param>
		///
		public static void drawAxis(int axis, int maxTics, int ticLength, float lowVal, float highVal, int screenStart, int screenEnd, int screenOffset, bool logScale, int screenHeight, Graphics g)
		{
			if(logScale && (lowVal == 0 || highVal == 0))
			{
				throw new System.ArgumentException("Axis.drawAxis: zero range value not allowed in log axes");
			}

			Color textColor = Color.Black;
			Pen pen = new Pen(textColor, 1);
			Font font = new Font("Arial", 7);
			SolidBrush brush = new SolidBrush(pen.Color);
			
			if(axis == X_AXIS) // horizontal baseline
			{
				g.DrawLine(pen, screenStart, screenHeight-screenOffset, screenEnd, screenHeight-screenOffset);
			}
			else // vertical baseline
			{
				g.DrawLine(pen, screenOffset, screenStart, screenOffset, screenEnd);
			}
			ArrayList tics = Axis.computeTicks(lowVal, highVal, maxTics); // nice round numbers for tic labels
			int last_label_end = axis == X_AXIS ? -88888 : 88888;
			string dbgstr = "tics:    ";
			for(IEnumerator e=tics.GetEnumerator(); e.MoveNext();)
			{
				string ticstr = (string)e.Current;
				if(DEBUG)
				{
					dbgstr += ticstr + ", ";
				}
				float ticval = Convert.ToSingle(ticstr);
				int tic_coord = screenStart;
				SizeF str_size = stringSize(ticstr, font, g);
				tic_coord += plotValue(ticval, lowVal, highVal, screenStart, screenEnd, logScale, screenHeight);
				if (axis == X_AXIS) // horizontal axis == vertical tics
				{
					g.DrawLine(pen, tic_coord, screenHeight-screenOffset, tic_coord, screenHeight-screenOffset+ticLength);
					if (tic_coord-str_size.Width/2 > last_label_end)
					{
						g.DrawString(ticstr, font, brush, tic_coord-str_size.Width/2, screenHeight-screenOffset+str_size.Height+5);
						last_label_end = (int)(tic_coord + str_size.Width/2 + str_size.Height/2);
					}
				}
				else // vertical axis == horizontal tics
				{
					tic_coord = screenHeight - tic_coord; // flips Y coordinates
					g.DrawLine(pen, screenOffset-ticLength, tic_coord, screenOffset, tic_coord);
					if (tic_coord-str_size.Height/3 < last_label_end)
					{
						g.DrawString(ticstr, font, brush, screenOffset-ticLength-str_size.Width-5, tic_coord+str_size.Height/3);
						last_label_end = (int)(tic_coord - str_size.Height);
					}
				}
			}
			if(DEBUG)
			{
				System.Diagnostics.Debug.WriteLine(dbgstr);
			}
		} // end drawAxis

		///
		/// <summary> * lower level method to determine a screen location where a given value
		/// * should be plotted given range, type, and screen information.
		/// * the "val" parameter is the data value to be plotted
		/// * @author Melinda Green </summary>
		/// * <param name="val"> is a data value to be plotted. </param>
		/// * <returns> pixel offset (row or column) to draw a screen representation
		/// * of the given data value. i.e. <i>where</i>  along an axis
		/// * in screen coordinates the caller should draw a representation of
		/// * the given value. </returns>
		/// * <seealso cref= drawAxis(int,int,int,float,float,int,int,int,boolean,int,Graphics) </seealso>
		///
		public static int plotValue(float val, float lowVal, float highVal, int screenStart, int screenEnd, bool logScale, int screenHeight)
		{
			if(logScale && (lowVal == 0 || highVal == 0 || val == 0))
			{
				throw new System.ArgumentException("Axis.drawAxis: zero range value not allowed in log axes");
			}
			int screen_range = screenEnd - screenStart; // in pixels
			if (logScale)
			{
				float log_low = flog10(lowVal), log_high = flog10(highVal), log_val = flog10(val);
				float log_range = log_high - log_low;
				float pixels_per_log_unit = screen_range / log_range;
				return (int)((log_val - log_low) * pixels_per_log_unit +.5);
			}
			else
			{
				float value_range = highVal - lowVal; // in data value units
				float pixels_per_unit = screen_range / value_range;
				return (int)((val-lowVal) * pixels_per_unit +.5);
			}
		}

		/*
		 * Given a number, round up to the nearest power of ten
		 * times 1, 2, or 5.
		 *
		 * Note: The argument must be strictly positive.
		 */
		private static double roundUp(double val)
		{
			int exponent = (int) Math.Floor(log10(val));
			val *= Math.Pow(10, -exponent);
			if (val > 5.0)
			{
				val = 10.0;
			}
			else if (val > 2.0)
			{
				val = 5.0;
			}
			else if (val > 1.0)
			{
				val = 2.0;
			}
			val *= Math.Pow(10, exponent);
			return val;
		}

		/*
		 * Return the number of fractional digits required to display the
		 * given number. No number larger than 15 is returned (if
		 * more than 15 digits are required, 15 is returned).
		 */
		private static int numFracDigits(double num)
		{
			int numdigits = 0;
			while (numdigits <= 15 && num != Math.Floor(num))
			{
				num *= 10.0;
				numdigits += 1;
			}
			return numdigits;
		}

		/*
		 * Return a string for displaying the specified number
		 * using the specified number of digits after the decimal point.
		 */
		private static string formatNum(double num, int numfracdigits)
		{
			return Math.Round(num,numfracdigits).ToString();
		}

		///
		/// <summary> * handy little utility for determining the length in pixels the
		/// * given string will use if drawn into the given Graphics object.
		/// * Note: perhaps belongs in some utility package. </summary>
		///
		public static SizeF stringSize(string str, Font stringFont, Graphics g)
		{
			SizeF size = new SizeF();
			if (g != null)
			{
				size = g.MeasureString(str, stringFont);
			}
			return size;
		}
	}
	
	/*
	//
	// TEST CODE FROM HERE DOWN
	//
	private class TestPanel : JPanel
	{
		private float curLowVal, curHighVal;
		private bool logScale;
		public TestPanel(float initialLow, float initialHigh)
		{
			curLowVal = initialLow;
			curHighVal = initialHigh;
		}
		public virtual bool LogScale
		{
			set
			{
				this.logScale = value;
				repaint();
			}
		}
		public virtual float Low
		{
			set
			{
				curLowVal = value;
				repaint();
			}
		}
		public virtual float High
		{
			set
			{
				curHighVal = value;
				repaint();
			}
		}
		public virtual void paint(Graphics g)
		{
			base.paint(g);
			drawAxis(Axis.X_AXIS, 10, 5, curLowVal, curHighVal, 50, Width-50, 50, logScale, Height, g);
			drawAxis(Axis.Y_AXIS, 10, 5, curLowVal, curHighVal, 50, Height-50, 50, logScale, Height, g);
			g.drawString("Current Slider Range: " + curLowVal + " --> " + curHighVal, 10, 20);
		}

		private static void addField(Container into, Component c, GridBagConstraints gbc, int x, int y, int w, int h, int wx, int wy)
		{
			gbc.gridx = x;
			gbc.gridy = y;
			gbc.gridwidth = w;
			gbc.gridheight = h;
			gbc.weightx = wx;
			gbc.weighty = wy;
			into.add(c, gbc);
		}

		///
		/// <summary> * simple example program for Axis class. </summary>
		///
		static void Main(string[] args)
		{
			const float INITIAL_MIN_LOW =1, INITIAL_MAX_HIGH =1000;
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final TestPanel axis = new TestPanel(INITIAL_MIN_LOW, INITIAL_MAX_HIGH);
			TestPanel axis = new TestPanel(INITIAL_MIN_LOW, INITIAL_MAX_HIGH);
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final JFrame frame = new JFrame("Axis Test");
			JFrame frame = new JFrame("Axis Test");
			JPanel mainpanel = new JPanel(new BorderLayout());
			mainpanel.add("Center", axis);
			JPanel controls = new JPanel();
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final JCheckBox logScale = new JCheckBox("Log");
			JCheckBox logScale = new JCheckBox("Log");
			logScale.addActionListener(new ActionListener() { public void actionPerformed(ActionEvent ae) { axis.setLogScale(logScale.Selected); } });
			logScale.Selected = true;
			axis.LogScale = true;
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final JTextField minlow = new JTextField(""+INITIAL_MIN_LOW, 6);
			JTextField minlow = new JTextField(""+INITIAL_MIN_LOW, 6);
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final FloatSlider lowSlider = new FloatSlider(JSlider.HORIZONTAL, INITIAL_MIN_LOW, 100, 1, 1000, 2000, false);
			FloatSlider lowSlider = new FloatSlider(JSlider.HORIZONTAL, INITIAL_MIN_LOW, 100, 1, 1000, 2000, false);
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final FloatSlider highSlider = new FloatSlider(JSlider.HORIZONTAL, INITIAL_MAX_HIGH, 100, 1, 1000, 2000, false);
			FloatSlider highSlider = new FloatSlider(JSlider.HORIZONTAL, INITIAL_MAX_HIGH, 100, 1, 1000, 2000, false);
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final JTextField maxhigh = new JTextField(""+INITIAL_MAX_HIGH, 6);
			JTextField maxhigh = new JTextField(""+INITIAL_MAX_HIGH, 6);
			GridBagLayout gridbag = new GridBagLayout();
			controls.Layout = gridbag;
			GridBagConstraints con = new GridBagConstraints();
			con.fill = GridBagConstraints.BOTH;
			con.insets = new Insets(0, 10, 0, 0);
			addField(controls, logScale, con, 0, 0, 1, 1, 5, 100);
			addField(controls, new JLabel("min low value"), con, 1, 0, 1, 1, 10, 100);
			addField(controls, minlow, con, 2, 0, 1, 1, 10, 100);
			addField(controls, lowSlider, con, 3, 0, 1, 1, 50, 100);
			addField(controls, highSlider, con, 4, 0, 1, 1, 50, 100);
			addField(controls, new JLabel("max high value"), con, 5, 0, 1, 1, 10, 100);
			addField(controls, maxhigh, con, 6, 0, 1, 1, 10, 100);
			mainpanel.add("South", controls);
			//JAVA TO C# CONVERTER TODO TASK: Anonymous inner classes are not converted to .NET:
//		KeyListener limitsWatcher = new KeyAdapter()
//		{
//			public void keyTyped(KeyEvent ke)
//			{
//				if(ke.getKeyChar() == KeyEvent.VK_ENTER)
//				{
//					float newminlow = Float.parseFloat(minlow.getText());
//					float newmaxhigh = Float.parseFloat(maxhigh.getText());
//					lowSlider.setAll(newminlow, newmaxhigh, lowSlider.getFloatValue());
//					highSlider.setAll(newminlow, newmaxhigh, highSlider.getFloatValue());
//				}
//			}
//		};
			minlow.addKeyListener(limitsWatcher);
			maxhigh.addKeyListener(limitsWatcher);
			//JAVA TO C# CONVERTER TODO TASK: Anonymous inner classes are not converted to .NET:
//		AdjustmentListener slider_watcher = new AdjustmentListener()
//		{
//			public void adjustmentValueChanged(AdjustmentEvent ae)
//			{
//				if (ae.getSource() == lowSlider)
//					axis.setLow((float)lowSlider.getFloatValue());
//				else
//					axis.setHigh((float)highSlider.getFloatValue());
//			}
//		};
			lowSlider.addAdjustmentListener(slider_watcher);
			highSlider.addAdjustmentListener(slider_watcher);
			frame.ContentPane.add(mainpanel);
			frame.Size = new Dimension(1000, 400);
			frame.DefaultCloseOperation = JFrame.EXIT_ON_CLOSE;
			frame.Visible = true;
		} // end main
	}

	 */
}
