///
/// <summary> * Audalyzer: an audio analyzer for Android.
/// * <br>Copyright 2009-2010 Ian Cameron Smith
/// *
/// * <p>This program is free software; you can redistribute it and/or modify
/// * it under the terms of the GNU General Public License version 2
/// * as published by the Free Software Foundation (see COPYING).
/// * 
/// * <p>This program is distributed in the hope that it will be useful,
/// * but WITHOUT ANY WARRANTY; without even the implied warranty of
/// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
/// * GNU General Public License for more details. </summary>
/// 

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Wave2Zebra2Preset.HermitGauges
{
	///
	/// <summary> * The main audio analyser view. </summary>
	/// 
	public class InstrumentPanel
	{

		// ******************************************************************** //
		// Public Constants.
		// ******************************************************************** //

		///
		/// <summary> * Definitions of the available window functions. </summary>
		///
		public enum Instruments
		{
			/// <summary> Spectrum Gauge, Power and Wave. </summary>
			SPECTRUM_P_W,

			/// <summary> Sonagram Gauge, Power and Wave. </summary>
			SONAGRAM_P_W,

			/// <summary> Spectrum and Sonagram Gauge. </summary>
			SPECTRUM_SONAGRAM,

			/// <summary> Spectrum Gauge. </summary>
			SPECTRUM,

			/// <summary> Sonagram Gauge. </summary>
			SONAGRAM,
		}


		// ******************************************************************** //
		// Constructor.
		// ******************************************************************** //

		///
		///	 <summary> * Create a AudioAnalyser instance.</summary>
		///				
		public InstrumentPanel()
		{
			audioAnalyser = new AudioAnalyser();
		}

		///
		///	 <summary> * Create a AudioAnalyser instance.</summary>
		/// * <param name="width">  The new width of the surface. </param>
		/// * <param name="height"> The new height of the surface. </param>
		///				
		public InstrumentPanel(int width, int height)
		{
			//Save current layout
			currentWidth=width;
			currentHeight=height;
			audioAnalyser = new AudioAnalyser();
		}
		
		
		///
		/// <summary> * Get the Audio Analyser.
		/// *  </summary>
		///
		public virtual AudioAnalyser AudioAnalyser 
		{
			get
			{
				return audioAnalyser;
			}
		}		
		
		// ******************************************************************** //
		// Configuration.
		// ******************************************************************** //

		///
		/// <summary> * Set the sample rate for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The desired rate, in samples/sec. </param>
		///
		public virtual int SampleRate
		{
			set
			{
				audioAnalyser.SampleRate = value;
			}
		}


		///
		/// <summary> * Set the input block size for this instrument.
		/// *  </summary>
		/// * <param name="size"> The desired block size, in samples. </param>
		///
		public virtual int BlockSize
		{
			set
			{
				audioAnalyser.BlockSize = value;
			}
		}


		///
		/// <summary> * Set the spectrum analyser windowing function for this instrument.
		/// *  </summary>
		/// * <param name="func"> The desired windowing function.
		/// *                      Window.Function.BLACKMAN_HARRIS is a good option.
		/// *                      Window.Function.RECTANGULAR turns off windowing. </param>
		///
		public virtual Window.Function WindowFunc
		{
			set
			{
				audioAnalyser.WindowFunc = value;
			}
		}


		///
		/// <summary> * Set the decimation rate for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The desired decimation. Only 1 in rate blocks
		/// *                      will actually be processed. </param>
		///
		public virtual int Decimation
		{
			set
			{
				audioAnalyser.Decimation = value;
			}
		}

		///
		/// <summary> * Set the histogram averaging window for this instrument.
		/// *  </summary>
		/// * <param name="rate"> The averaging interval. 1 means no averaging. </param>
		///
		public virtual int AverageLen
		{
			set
			{
				audioAnalyser.AverageLen = value;
			}
		}

		///
		/// <summary> * Set the instruments to display
		/// *  </summary>
		/// * <param name="InstrumentPanel.Intruments"> Choose which ones to display. </param>
		///
		public InstrumentPanel.Instruments Instrument
		{
			set
			{
				currentInstruments=value;
				loadInstruments(currentInstruments);
			}
		}				

		///
		/// <summary> * Load instruments
		/// *  </summary>
		/// * <param name="InstrumentPanel.Intruments"> Choose which ones to display. </param>
		///
		private void loadInstruments(InstrumentPanel.Instruments i)
		{
			//Log.i(TAG, "Load instruments");

			//Stop surface update
			//onPause();

			//Clear surface events
			//clearGauges();

			//Clear analyse events
			audioAnalyser.resetGauge();

			//Destroy last Gauges
			sonagramGauge=null;
			spectrumGauge=null;
			powerGauge=null;
			waveformGauge=null;

			//Create instruments, update and refresh
			if ((i==InstrumentPanel.Instruments.SPECTRUM)||(i==InstrumentPanel.Instruments.SPECTRUM_SONAGRAM)||(i==InstrumentPanel.Instruments.SPECTRUM_P_W))
			{
				spectrumGauge = audioAnalyser.getSpectrumGauge();
			}

			if ((i==InstrumentPanel.Instruments.SONAGRAM)||(i==InstrumentPanel.Instruments.SPECTRUM_SONAGRAM)||(i==InstrumentPanel.Instruments.SONAGRAM_P_W))
			{
				sonagramGauge = audioAnalyser.getSonagramGauge();
			}

			if ((i==InstrumentPanel.Instruments.SPECTRUM_P_W)||(i==InstrumentPanel.Instruments.SONAGRAM_P_W))
			{
				waveformGauge = audioAnalyser.getWaveformGauge();

				powerGauge = audioAnalyser.getPowerGauge();
			}

			//Load current layout in Gauges if they're already define
			if ((currentWidth>0)&&(currentHeight>0))
			{
				refreshLayout();
			}

			//Restart
			//onResume();

			//Log.i(TAG, "End instruments loading");
		}


		// ******************************************************************** //
		// Layout Processing.
		// ******************************************************************** //

		///
		/// <summary> * Lay out the display for a given screen size.
		/// *  </summary>
		/// * <param name="width">  The new width of the surface. </param>
		/// * <param name="height"> The new height of the surface. </param>
		///
		protected internal void layout(int width, int height)
		{
			//Save current layout
			currentWidth=width;
			currentHeight=height;
			refreshLayout();
		}

		///
		/// <summary> * Lay out the display for the current screen size. </summary>
		///
		protected internal virtual void refreshLayout()
		{
			// Make up some layout parameters.
			int min = Math.Min(currentWidth, currentHeight);
			int gutter = min / (min > 400 ? 15 : 20);

			// Calculate the layout based on the screen configuration.
			if (currentWidth > currentHeight)
			{
				layoutLandscape(currentWidth, currentHeight, gutter);
			}
			else
			{
				layoutPortrait(currentWidth, currentHeight, gutter);
			}

			// Set the gauge geometries.
			if (waveformGauge!=null)
			{
				waveformGauge.Geometry = waveRect;
			}
			if (spectrumGauge!=null)
			{
				spectrumGauge.Geometry = specRect;
			}
			if (sonagramGauge!=null)
			{
				sonagramGauge.Geometry = sonaRect;
			}
			if (powerGauge!=null)
			{
				powerGauge.Geometry = powerRect;
			}
		}

		///
		/// <summary> * Lay out the display for a given screen size.
		/// *  </summary>
		/// * <param name="width">  The new width of the surface. </param>
		/// * <param name="height"> The new height of the surface. </param>
		/// * <param name="gutter"> Spacing to leave between items. </param>
		///
		private void layoutLandscape(int width, int height, int gutter)
		{
			int x = gutter;
			int y = gutter;

			// Divide the display into two columns.
			int col = (width - gutter * 3) / 2;

			//Init
			waveRect = new Rectangle(0,0,0,0);
			specRect = new Rectangle(0,0,0,0);
			sonaRect = new Rectangle(0,0,0,0);
			powerRect = new Rectangle(0,0,0,0);

			if (waveformGauge!=null)
			{
				// Divide the left pane in two.
				int row = (height - gutter * 3) / 2;

				//Wave+Spectrum+Power or Wave+Sonagram+Power
				waveRect = new Rectangle(x, y, x + col, y + row);
				y += row + gutter;
				powerRect = new Rectangle(x, y, x + col, height - gutter);
				x += col + gutter;
				y = gutter;

				//Spectrum or Sonagram fullscreen
				if (spectrumGauge!=null)
				{
					specRect = new Rectangle(x, y, x + col, height - gutter);
				}
				else
				{
					sonaRect = new Rectangle(x, y, x + col, height - gutter);
				}
			}
			else if ((spectrumGauge!=null)&&(sonagramGauge!=null))
			{
				//Spectrum + Sonagram
				specRect = new Rectangle(x, y, x + col, height - gutter);
				x += col + gutter;
				sonaRect = new Rectangle(x, y, x + col, height - gutter);
			}
			else
			{
				//Spectrum or Sonagram fullscreen
				if (spectrumGauge!=null)
				{
					specRect = new Rectangle(x, y, width - gutter, height - gutter);
				}
				else
				{
					sonaRect = new Rectangle(x, y, width - gutter, height - gutter);
				}
			}
		}

		///
		/// <summary> * Lay out the display for a given screen size.
		/// *  </summary>
		/// * <param name="width">  The new width of the surface. </param>
		/// * <param name="height"> The new height of the surface. </param>
		/// * <param name="gutter"> Spacing to leave between items. </param>
		///
		private void layoutPortrait(int width, int height, int gutter)
		{
			int x = gutter;
			int y = gutter;

			// Display one column.
			int col = width - gutter * 2;

			//Init
			waveRect = new Rectangle(0,0,0,0);
			specRect = new Rectangle(0,0,0,0);
			sonaRect = new Rectangle(0,0,0,0);
			powerRect = new Rectangle(0,0,0,0);

			if (waveformGauge!=null)
			{
				// Divide the display into three vertical elements, the
				// spectrum or sonagram display being double-height.
				int unit = (height - gutter * 4) / 4;

				//Wave+Spectrum+Power or Wave+Sonagram+Power
				waveRect = new Rectangle(x, y, x + col, y + unit);
				y += unit + gutter;

				if (spectrumGauge!=null)
				{
					specRect = new Rectangle(x, y, x + col, y + unit * 2);
				}
				else
				{
					sonaRect = new Rectangle(x, y, x + col, y + unit * 2);
				}

				y += unit * 2 + gutter;
				powerRect = new Rectangle(x, y, x + col, y + unit);
			}
			else if ((spectrumGauge!=null)&&(sonagramGauge!=null))
			{
				// Divide the display into two vertical elements
				int unit = (height - gutter * 3) / 2;

				//Spectrum + Sonagram
				specRect = new Rectangle(x, y, x + col, y + unit);
				y += unit + gutter;
				sonaRect = new Rectangle(x, y, x + col, y + unit);
			}
			else
			{
				//Spectrum or Sonagram fullscreen
				if (spectrumGauge!=null)
				{
					specRect = new Rectangle(x, y, width - gutter, height - gutter);
				}
				else
				{
					sonaRect = new Rectangle(x, y, width - gutter, height - gutter);
				}
			}
		}


		// ******************************************************************** //
		// Input Handling.
		// ******************************************************************** //


		// ******************************************************************** //
		// Class Data.
		// ******************************************************************** //

		
		// Debugging tag.
		private const string TAG = "Audalyzer";


		// ******************************************************************** //
		// Private Data.
		// ******************************************************************** //

		// The current Intruments in pref.
		private Instruments currentInstruments = InstrumentPanel.Instruments.SPECTRUM;

		//Current layout
		private int currentWidth = 0;
		private int currentHeight = 0;

		// Our audio input device.
		private readonly AudioAnalyser audioAnalyser;

		// The gauges associated with this instrument.
		private WaveformGauge waveformGauge = null;
		private SpectrumGauge spectrumGauge = null;
		private SonagramGauge sonagramGauge = null;
		private PowerGauge powerGauge = null;

		// Bounding rectangles for the waveform, spectrum, sonagram, and VU meter displays.
		private Rectangle waveRect;
		private Rectangle specRect;
		private Rectangle sonaRect;
		private Rectangle powerRect;
	}
}