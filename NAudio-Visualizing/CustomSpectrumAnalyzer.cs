using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CommonUtils.Audio;

namespace NAudio_Visualizing
{
	/// <summary>
	/// Description of CustomSpectrumAnalyzer.
	/// </summary>
	public partial class CustomSpectrumAnalyzer : UserControl
	{
		#region Enums
		public enum BarHeightScalingStyles
		{
			/// <summary>
			/// A decibel scale. Formula: 20 * Log10(FFTValue). Total bar height
			/// is scaled from -90 to 0 dB.
			/// </summary>
			Decibel,

			/// <summary>
			/// A non-linear squareroot scale. Formula: Sqrt(FFTValue) * 2 * BarHeight.
			/// </summary>
			Sqrt,

			/// <summary>
			/// A linear scale. Formula: 9 * FFTValue * BarHeight.
			/// </summary>
			Linear
		}
		#endregion
		
		#region Fields
		private readonly Timer animationTimer;
		private ISpectrumPlayer soundPlayer;
		private Bitmap offlineBitmap;
		private readonly List<Rectangle> barShapes = new List<Rectangle>();
		private readonly List<Rectangle> peakShapes = new List<Rectangle>();
		private float[] channelData = new float[8192]; // store FFT results here
		private float[] channelPeakData;
		private double bandWidth = 1.0;
		private double barWidth = 1;
		private int maximumFrequencyIndex = 8191; // one less than fftDataSize
		private int minimumFrequencyIndex;
		private int[] barIndexMax;
		private int[] barLogScaleIndexMax;
		
		// Public fields
		public BarHeightScalingStyles BarHeightScaling = BarHeightScalingStyles.Decibel;
		public bool IsFrequencyScaleLinear = false;
		public bool AveragePeaks = false;
		public int PeakFallDelay = 10;
		public double BarSpacing = 5.0d;
		public int BarCount = 32;
		public int MaximumFrequency = 20000;
		public int MinimumFrequency = 20;
		public double ActualBarWidth = 0.0d;
		public bool DoSpectrumGraph = true;
		#endregion

		#region Constants
		private const int scaleFactorLinear = 9;
		private const int scaleFactorSqr = 2;
		private const double minDBValue = -90;
		private const double maxDBValue = 0;
		private const double dbScale = (maxDBValue - minDBValue);
		private const int defaultUpdateInterval = 25;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new SpectrumAnalyzer control
		/// </summary>
		public CustomSpectrumAnalyzer()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			              ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			this.DoubleBuffered = true;

			animationTimer = new Timer()
			{
				Interval = defaultUpdateInterval,
			};
			animationTimer.Tick += animationTimer_Tick;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Register a sound player from which the spectrum analyzer
		/// can get the necessary playback data.
		/// </summary>
		/// <param name="soundPlayer">A sound player that provides spectrum data through the ISpectrumPlayer interface methods.</param>
		public void RegisterSoundPlayer(ISpectrumPlayer soundPlayer)
		{
			this.soundPlayer = soundPlayer;
			soundPlayer.PropertyChanged += soundPlayer_PropertyChanged;
			UpdateBarLayout();
			animationTimer.Start();
		}
		#endregion

		#region Event Overrides
		protected override void OnPaint(PaintEventArgs e)
		{
			if (DoSpectrumGraph) {
				if (offlineBitmap != null) {
					e.Graphics.DrawImage(offlineBitmap, 0, 0);
				}
			} else {
				Color lineColor = ColorTranslator.FromHtml("#C7834C");
				Color middleLineColor = ColorTranslator.FromHtml("#EFAB74");
				Color textColor = ColorTranslator.FromHtml("#A9652E");
				Color sampleColor = ColorTranslator.FromHtml("#4C2F1A");
				Color fillOuterColor = ColorTranslator.FromHtml("#FFFFFF");
				Color fillColor = ColorTranslator.FromHtml("#F9C998");
				
				if (barShapes.Count > 0 && peakShapes.Count > 0) {
					Pen linePen = new Pen(lineColor, 0.5f);
					Brush fillBrush = new SolidBrush(fillColor);
					Brush sampleBrush = new SolidBrush(sampleColor);
					e.Graphics.FillRectangle(fillBrush, 0, 0, this.Width, this.Height);
					e.Graphics.DrawRectangle(linePen, 0, 0, this.Width - 1, this.Height - 1);

					e.Graphics.FillRectangles(sampleBrush, barShapes.ToArray());
					e.Graphics.FillRectangles(Brushes.DeepSkyBlue, peakShapes.ToArray());
				}
			}
			// Calling the base class OnPaint
			base.OnPaint(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateBarLayout();
		}
		#endregion

		#region Private Drawing Methods
		private void UpdateSpectrum()
		{
			if (soundPlayer == null)
				return;

			if (soundPlayer.IsPlaying && !soundPlayer.GetFFTData(channelData))
				return;

			UpdateSpectrumShapes();
			
			// force redraw
			this.Invalidate();
		}

		/// <summary>
		/// Use the FFT results to update the height and y position of the bars
		/// </summary>
		private void UpdateSpectrumShapes()
		{
			bool allZero = true;
			if (DoSpectrumGraph) {
				float[] mag;
				float[] freq;
				float foundMaxFreq, foundMaxDecibel;
				double sampleRate = soundPlayer.SampleRate;
				int fftWindowsSize = soundPlayer.FftDataSize;
				CommonUtils.FFT.AudioAnalyzer.PrepareSpectrumAnalysis(channelData, sampleRate, fftWindowsSize, out mag, out freq, out foundMaxFreq, out foundMaxDecibel);
				this.offlineBitmap = CommonUtils.FFT.AudioAnalyzer.GetSpectrumImage(ref mag, ref freq, new Size(this.Width, this.Height), MinimumFrequency, MaximumFrequency, foundMaxDecibel, foundMaxFreq);
			} else {
				double fftBucketHeight = 0f;
				double barHeight = 0f;
				double lastPeakHeight = 0f;
				double peakYPos = 0f;
				double height = this.Height;
				int barIndex = 0;
				double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
				double barHeightScale = (height - peakDotHeight);

				for (int i = minimumFrequencyIndex; i <= maximumFrequencyIndex; i++)
				{
					// If we're paused, keep drawing, but set the current height to 0 so the peaks fall.
					if (!soundPlayer.IsPlaying)
					{
						barHeight = 0f;
					}
					else // Draw the maximum value for the bar's band
					{
						switch (BarHeightScaling)
						{
							case BarHeightScalingStyles.Decibel:
								double dbValue = 20 * Math.Log10((double)channelData[i]);
								fftBucketHeight = ((dbValue - minDBValue) / dbScale) * barHeightScale;
								break;
							case BarHeightScalingStyles.Linear:
								fftBucketHeight = (channelData[i] * scaleFactorLinear) * barHeightScale;
								break;
							case BarHeightScalingStyles.Sqrt:
								fftBucketHeight = (((Math.Sqrt((double)channelData[i])) * scaleFactorSqr) * barHeightScale);
								break;
						}

						if (barHeight < fftBucketHeight)
							barHeight = fftBucketHeight;
						if (barHeight < 0f)
							barHeight = 0f;
					}

					// If this is the last FFT bucket in the bar's group, draw the bar.
					int currentIndexMax = IsFrequencyScaleLinear ? barIndexMax[barIndex] : barLogScaleIndexMax[barIndex];
					if (i == currentIndexMax)
					{
						// Peaks can't surpass the height of the control.
						if (barHeight > height)
							barHeight = height;

						if (AveragePeaks && barIndex > 0)
							barHeight = (lastPeakHeight + barHeight) / 2;

						peakYPos = barHeight;

						if (channelPeakData[barIndex] < peakYPos) {
							channelPeakData[barIndex] = (float)peakYPos;
						} else {
							channelPeakData[barIndex] = (float)(peakYPos + (PeakFallDelay * channelPeakData[barIndex])) / ((float)(PeakFallDelay + 1));
						}

						double xCoord = BarSpacing + (barWidth * barIndex) + (BarSpacing * barIndex) + 1;

						Rectangle barRect = barShapes[barIndex];
						barRect.Y = (int)((height - 1) - barHeight);
						barRect.Height = (int)barHeight;
						barShapes[barIndex] = barRect;
						
						Rectangle peakRect = peakShapes[barIndex];
						peakRect.Y = (int)((height - 1) - channelPeakData[barIndex] - peakDotHeight);
						peakShapes[barIndex] = peakRect;

						if (channelPeakData[barIndex] > 0.05)
							allZero = false;

						lastPeakHeight = barHeight;
						barHeight = 0f;
						barIndex++;
					}
				}
			}

			if (allZero && !soundPlayer.IsPlaying)
				animationTimer.Stop();
		}

		/// <summary>
		/// Create and layout the bars
		/// </summary>
		private void UpdateBarLayout()
		{
			if (soundPlayer == null)
				return;

			if (DoSpectrumGraph) {
				UpdateSpectrumShapes();
				return;
			}
			
			barWidth = Math.Max(((double)(this.Width - (BarSpacing * (BarCount + 1))) / (double)BarCount), 1);
			maximumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MaximumFrequency) + 1, 8191);
			minimumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MinimumFrequency), 8191);
			bandWidth = Math.Max(((double)(maximumFrequencyIndex - minimumFrequencyIndex)) / this.Width, 1.0);

			int actualBarCount;
			if (barWidth >= 1.0d) {
				actualBarCount = BarCount;
			} else {
				actualBarCount = Math.Max((int)((this.Width - BarSpacing) / (barWidth + BarSpacing)), 1);
			}
			channelPeakData = new float[actualBarCount];

			int indexCount = maximumFrequencyIndex - minimumFrequencyIndex;
			int linearIndexBucketSize = (int)Math.Round((double)indexCount / (double)actualBarCount, 0);
			List<int> maxIndexList = new List<int>();
			List<int> maxLogScaleIndexList = new List<int>();
			double maxLog = Math.Log(actualBarCount, actualBarCount);
			for (int i = 1; i < actualBarCount; i++)
			{
				maxIndexList.Add(minimumFrequencyIndex + (i * linearIndexBucketSize));
				int logIndex = (int)((maxLog - Math.Log((actualBarCount + 1) - i, (actualBarCount + 1))) * indexCount) + minimumFrequencyIndex;
				maxLogScaleIndexList.Add(logIndex);
			}
			maxIndexList.Add(maximumFrequencyIndex);
			maxLogScaleIndexList.Add(maximumFrequencyIndex);
			barIndexMax = maxIndexList.ToArray();
			barLogScaleIndexMax = maxLogScaleIndexList.ToArray();
			
			barShapes.Clear();
			peakShapes.Clear();

			double height = this.Height;
			double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
			for (int i = 0; i < actualBarCount; i++)
			{
				double xCoord = BarSpacing + (barWidth * i) + (BarSpacing * i) + 1;
				Rectangle barRectangle = new Rectangle()
				{
					X = (int) xCoord,
					Y = (int) height,
					Width = (int) barWidth,
					Height = 0,
				};
				barShapes.Add(barRectangle);
				Rectangle peakRectangle = new Rectangle()
				{
					X = (int) xCoord,
					Y = (int) (height - peakDotHeight),
					Width = (int) barWidth,
					Height = (int) peakDotHeight,
				};
				peakShapes.Add(peakRectangle);
			}
			
			ActualBarWidth = barWidth;
		}
		#endregion

		#region Event Handlers
		private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "IsPlaying":
					if (soundPlayer.IsPlaying && !animationTimer.Enabled)
						animationTimer.Start();
					break;
			}
		}

		private void animationTimer_Tick(object sender, EventArgs e)
		{
			UpdateSpectrum();
		}
		#endregion
	}
}
