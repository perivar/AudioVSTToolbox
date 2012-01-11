using System;
using System.ComponentModel;

using System.Drawing;
using System.Drawing.Imaging;

using System.Windows.Forms;

using CommonUtils.FFT;

namespace CommonUtils.GUI
{
	/// <summary>
	/// Description of WaveDisplayUserControl.
	/// </summary>
	public partial class WaveDisplayUserControl : UserControl
	{
		private Bitmap bmp;
		private int waveDisplayResolution = 1;
		
		public int Resolution
		{
			set {
				waveDisplayResolution = value;
			}
			get {
				return waveDisplayResolution;
			}
		}
		
		public WaveDisplayUserControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
			              ControlStyles.OptimizedDoubleBuffer, true);
			InitializeComponent();
			
		}
		
		/// <summary>
		/// <see cref="Control.OnPaint"/>
		/// </summary>
		protected override void OnPaint(PaintEventArgs pe)
		{
			if (bmp != null) {
				pe.Graphics.DrawImage(bmp, 0, 0);
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
		
		private float[] audioData;

		/// <summary>
		/// sets graph data
		/// </summary>
		public void SetAudioData(float[] audioData)
		{
			this.audioData = audioData;

			bmp = AudioAnalyzer.DrawWaveform(audioData,
			                                 new Size(this.Width, this.Height),
			                                 waveDisplayResolution);
			
			// force redraw
			this.Invalidate();
		}
	}
}
